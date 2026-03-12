using Microsoft.AspNetCore.Mvc;
using KMITL_WebDev_MiniProject.Models;
using KMITL_WebDev_MiniProject.Entites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KMITL_WebDev_MiniProject.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationActivitiesDbContext _activitiesContext;
    private readonly ApplicationUserUtilDbContext _userUtilContext;
    private readonly UserManager<UserAccount> _userManager;

    public HomeController(
        ApplicationActivitiesDbContext activitiesContext,
        ApplicationUserUtilDbContext userUtilContext,
        UserManager<UserAccount> userManager)
    {
        _activitiesContext = activitiesContext;
        _userUtilContext = userUtilContext;
        _userManager = userManager;
    }

    [HttpGet]
    [Authorize]
    [Route("Home")]	
    [Route("Home/Index")]	
    public async Task<IActionResult> Index()
    {
        var activities = await _activitiesContext.Activities
            .Include(a => a.ActivityUsers)
                .ThenInclude(au => au.User)
            .ToListAsync();
        var previews = new List<ActivityPreviewViewModel>(activities.Count);
        foreach (var act in activities)
        {
            var ownerName = string.Empty;
            if (act.OwnerId != Guid.Empty)
            {
                var owner = await _userManager.FindByIdAsync(act.OwnerId.ToString());
                if (owner != null)
                    ownerName = owner.RealUserName ?? owner.UserName ?? string.Empty;
            }
            previews.Add(new ActivityPreviewViewModel { Act = act, OwnerName = ownerName });
        }
        return View(previews);
    }

    [HttpGet]
    [Authorize]
    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Popular()
    {
        var likedPostRows = await _activitiesContext.Relations
            .Where(r => r.Relation == 1)
            .GroupBy(r => r.ActID)
            .Select(g => new { ActivityId = g.Key, LikeCount = g.Count() })
            .OrderByDescending(x => x.LikeCount)
            .Take(20)
            .ToListAsync();

        var activityIds = likedPostRows.Select(x => x.ActivityId).ToList();
        var activities = await _activitiesContext.Activities
            .Where(a => activityIds.Contains(a.Id))
            .Include(a => a.ActivityUsers)
                .ThenInclude(au => au.User)
            .ToListAsync();

        var ownerIds = activities
            .Where(a => a.OwnerId != Guid.Empty)
            .Select(a => a.OwnerId)
            .Distinct()
            .ToList();

        var ownerNames = await _userManager.Users
            .Where(u => ownerIds.Contains(u.Id))
            .Select(u => new { u.Id, Name = u.RealUserName ?? u.UserName ?? string.Empty })
            .ToDictionaryAsync(x => x.Id, x => x.Name);

        var activityLookup = activities.ToDictionary(a => a.Id, a => a);
        var popularPosts = likedPostRows
            .Where(row => activityLookup.ContainsKey(row.ActivityId))
            .Select(row =>
            {
                var act = activityLookup[row.ActivityId];
                ownerNames.TryGetValue(act.OwnerId, out var ownerName);
                return new PopularActivityItem
                {
                    Activity = act,
                    OwnerName = ownerName ?? string.Empty,
                    LikeCount = row.LikeCount
                };
            })
            .ToList();

        var popularPeopleRows = await _userUtilContext.ReputationRelations
            .GroupBy(r => r.UserObj)
            .Select(g => new
            {
                UserId = g.Key,
                ReputationScore = g.Sum(x => x.Relation),
                PositiveVotes = g.Count(x => x.Relation > 0)
            })
            .OrderByDescending(x => x.ReputationScore)
            .ThenByDescending(x => x.PositiveVotes)
            .Take(20)
            .ToListAsync();

        var popularUserIds = popularPeopleRows.Select(x => x.UserId).ToList();
        var users = await _userManager.Users
            .Where(u => popularUserIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u);

        var popularPeople = popularPeopleRows
            .Where(row => users.ContainsKey(row.UserId))
            .Select(row =>
            {
                var user = users[row.UserId];
                return new PopularUserItem
                {
                    UserId = user.Id,
                    UserName = user.RealUserName ?? user.UserName ?? "Unknown",
                    ImagePath = string.IsNullOrWhiteSpace(user.ImagePath) ? "image/UserProfile/guest_picture.jpg" : user.ImagePath,
                    ReputationScore = row.ReputationScore,
                    PositiveVotes = row.PositiveVotes
                };
            })
            .ToList();

        var vm = new PopularViewModel
        {
            PopularPosts = popularPosts,
            PopularPeople = popularPeople
        };

        return View(vm);
    }

    [HttpGet]
    [Authorize]
    public IActionResult ActivityDetail()
    {
        return View();
    }

    public IActionResult Profile()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [AllowAnonymous]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpGet]
    [Produces("application/json")] 
    [Authorize]
    public IActionResult GetSuggestion(string keyword)
    {   
        if(string.IsNullOrEmpty(keyword))
        {
            return Json(new List<string>());
        }
        var suggestions = _activitiesContext.Activities
            .Where(a => 
            a.Name.StartsWith(keyword) ||
            (a.KeywordsText != null && EF.Functions.Like(a.KeywordsText, $"%{keyword}%"))
            
            )
            .OrderBy(a => a.Name)
            .Select(a => a.Name)
            .Take(4)
            .ToList();
        return Json(suggestions);
    }
}

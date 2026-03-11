using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KMITL_WebDev_MiniProject.Entites;
using KMITL_WebDev_MiniProject.Models;
using Microsoft.AspNetCore.Identity;
using KMITL_WebDev_MiniProject.DTO;
using KMITL_WebDev_MiniProject.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace KMITL_WebDev_MiniProject.Controllers;

[Authorize]
[Route("Activity")]
public class ActivityController : Controller
{
    private readonly ApplicationActivitiesDbContext _activitiesContext;
    private readonly UserManager<UserAccount> _userManager;
    private readonly ApplicationUserUtilDbContext _UserUtilDbContext;
    private readonly CommentServices _ComSer;
    private readonly FileUploadServcies _fileUploader;

    public ActivityController(ApplicationActivitiesDbContext activitiesContext, ApplicationUserUtilDbContext UserUtilDbContext, UserManager<UserAccount> userManager, IWebHostEnvironment env)
    {
        _activitiesContext = activitiesContext;
        _userManager = userManager;
        _UserUtilDbContext = UserUtilDbContext;
        _ComSer = new CommentServices(_UserUtilDbContext, _userManager);
        _fileUploader = new FileUploadServcies(env);
    }

    [HttpGet("Create")]
    [HttpGet("createactivity")]
    public IActionResult Create()
    {
        var now = DateTime.Now;
        return View(new ActivityViewModel 
        { 
            EventDate = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0)
        });
    }

    [HttpPost("Create")]
    [HttpPost("createactivity")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ActivityViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        // determine owner from logged-in user
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Forbid();

        var activity = new Activity
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Description = model.Description,
            KeywordsText = string.Join(", ", (model.KeywordInput ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(k => k.Trim())
                .Where(k => !string.IsNullOrWhiteSpace(k))
                .Distinct(StringComparer.OrdinalIgnoreCase)),
            ImageUrl = model.ImageUrl,
            MaxPeople = model.MaxPeople,
            RecruitingMode = (int)model.RecruitingMode,
            ShowParticipants = model.ShowParticipants,
            OwnerId = user.Id,
            EventDate = model.EventDate,
            Location = model.Location,
            MapUrl = model.MapUrl,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _activitiesContext.Activities.Add(activity);
        await _activitiesContext.SaveChangesAsync();

        if (_fileUploader.FileIsExist(model.ActivityImage))
        {
            await _fileUploader.Upload(model.ActivityImage, activity.Id.ToString());
            activity.ImageUrl = Path.Combine("image", "UserProfile", activity.Id + _fileUploader.LastExt);
            _activitiesContext.Activities.Update(activity);
            await _activitiesContext.SaveChangesAsync();
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var activity = await _activitiesContext.Activities.FirstOrDefaultAsync(a => a.Id == id);
        if (activity == null)
            return NotFound();

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Challenge();

        if (activity.OwnerId != user.Id)
            return Forbid();

        var model = new ActivityViewModel
        {
            Id = activity.Id,
            Name = activity.Name,
            Description = activity.Description,
            KeywordInput = activity.KeywordsText,
            ImageUrl = activity.ImageUrl,
            MaxPeople = activity.MaxPeople,
            RecruitingMode = (RecruitingMode)activity.RecruitingMode,
            ShowParticipants = activity.ShowParticipants,
            EventDate = activity.EventDate,
            Location = activity.Location,
            MapUrl = activity.MapUrl
        };

        return View(model);
    }

    [HttpPost("Edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, ActivityViewModel model)
    {
        if (id != model.Id)
            return BadRequest();

        var activity = await _activitiesContext.Activities.FirstOrDefaultAsync(a => a.Id == id);
        if (activity == null)
            return NotFound();

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Challenge();

        if (activity.OwnerId != user.Id)
            return Forbid();

        if (!ModelState.IsValid)
            return View(model);

        activity.Name = model.Name;
        activity.Description = model.Description;
        activity.KeywordsText = string.Join(", ", (model.KeywordInput ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(k => k.Trim())
            .Where(k => !string.IsNullOrWhiteSpace(k))
            .Distinct(StringComparer.OrdinalIgnoreCase));
        activity.ImageUrl = model.ImageUrl;
        activity.MaxPeople = model.MaxPeople;
        activity.RecruitingMode = (int)model.RecruitingMode;
        activity.ShowParticipants = model.ShowParticipants;
        activity.EventDate = model.EventDate;
        activity.Location = model.Location;
        activity.MapUrl = model.MapUrl;
        activity.UpdatedAt = DateTime.Now;

        if (_fileUploader.FileIsExist(model.ActivityImage))
        {
            await _fileUploader.Upload(model.ActivityImage, activity.Id.ToString());
            activity.ImageUrl = Path.Combine("image", "UserProfile", activity.Id + _fileUploader.LastExt);
        }

        _activitiesContext.Activities.Update(activity);
        await _activitiesContext.SaveChangesAsync();

        return RedirectToAction(nameof(Detail), new { id = activity.Id });
    }

    [HttpGet("Detail/{id}")]
    [HttpGet("ActivityDetail/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> Detail(Guid id)
    {
        // include keywords so the view can render them
        Activity activity = await _activitiesContext.Activities
            .Where(a => a.Id == id)
            .Include(a => a.ActivityUsers)
                .ThenInclude(au => au.User)
            .FirstOrDefaultAsync();
        if (activity == null)
            return NotFound();

        // look up owner name
        string ownerName = "";
        string ownerImagePath = "image/UserProfile/guest_picture.jpg";
        if (activity.OwnerId != Guid.Empty)
        {
            var owner = await _userManager.FindByIdAsync(activity.OwnerId.ToString());
            if (owner != null)
            {
                ownerName = owner.RealUserName ?? owner.UserName;
                if (!string.IsNullOrWhiteSpace(owner.ImagePath))
                    ownerImagePath = owner.ImagePath;
            }
        }

        ActivityDTO dto = new ActivityDTO()
        {
            Act = activity,
            Comments = await _ComSer.ShowCommentDTOs(id),
            OwnerName = ownerName,
            OwnerImagePath = ownerImagePath
        };

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser != null)
        {
            dto.IsOwner = activity.OwnerId == currentUser.Id;
            dto.IsJoined = activity.Participants.Any(p => p.Id == currentUser.Id);
        }

        return View("ActivityDetail", dto);
    }

    [HttpPost("Join/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Join(Guid id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Challenge();

        var activity = await _activitiesContext.Activities
            .Include(a => a.ActivityUsers)
            .FirstOrDefaultAsync(a => a.Id == id);
        if (activity == null)
            return NotFound();

        if (activity.OwnerId == user.Id)
            return RedirectToAction(nameof(Detail), new { id });

        if (activity.ActivityUsers.Any(au => au.UserId == user.Id && au.Role == ActivityUserRole.Participant))
            return RedirectToAction(nameof(Detail), new { id });

        if (activity.ActivityUsers.Count(au => au.Role == ActivityUserRole.Participant) >= activity.MaxPeople)
            return RedirectToAction(nameof(Detail), new { id });

        activity.ActivityUsers.Add(new ActivityUser
        {
            ActivityId = activity.Id,
            UserId = user.Id,
            Role = ActivityUserRole.Participant
        });
        activity.UpdatedAt = DateTime.Now;
        await _activitiesContext.SaveChangesAsync();

        return RedirectToAction(nameof(Detail), new { id });
    }

    [HttpPost("Unjoin/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unjoin(Guid id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Challenge();

        var activity = await _activitiesContext.Activities
            .Include(a => a.ActivityUsers)
            .FirstOrDefaultAsync(a => a.Id == id);
        if (activity == null)
            return NotFound();

        if (activity.OwnerId == user.Id)
            return RedirectToAction(nameof(Detail), new { id });

        var participantRelation = activity.ActivityUsers
            .FirstOrDefault(au => au.UserId == user.Id && au.Role == ActivityUserRole.Participant);

        if (participantRelation == null)
            return RedirectToAction(nameof(Detail), new { id });

        activity.ActivityUsers.Remove(participantRelation);
        activity.UpdatedAt = DateTime.Now;
        await _activitiesContext.SaveChangesAsync();

        return RedirectToAction(nameof(Detail), new { id });
    }
}

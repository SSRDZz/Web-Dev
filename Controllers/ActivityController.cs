using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KMITL_WebDev_MiniProject.Entites;
using KMITL_WebDev_MiniProject.Models;
using Microsoft.AspNetCore.Identity;
using KMITL_WebDev_MiniProject.DTO;
using MvcMovie.Migrations.ApplicationUserUtil;
using KMITL_WebDev_MiniProject.Services;

namespace KMITL_WebDev_MiniProject.Controllers;

[Authorize]
[Route("Activity")]
public class ActivityController : Controller
{
    private readonly ApplicationActivitiesDbContext _activitiesContext;
    private readonly UserManager<UserAccount> _userManager;
    private readonly ApplicationUserUtilDbContext _UserUtilDbContext;
    private readonly CommentServices _ComSer;

    public ActivityController(ApplicationActivitiesDbContext activitiesContext, ApplicationUserUtilDbContext UserUtilDbContext, UserManager<UserAccount> userManager)
    {
        _activitiesContext = activitiesContext;
        _userManager = userManager;
        _UserUtilDbContext = UserUtilDbContext;
        _ComSer = new CommentServices(_UserUtilDbContext, _userManager);
    }

    [HttpGet("Create")]
    [HttpGet("createactivity")]
    public IActionResult Create()
    {
        return View(new ActivityViewModel());
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
            Name = model.Name,
            Description = model.Description,
            ImageUrl = model.ImageUrl,
            MaxPeople = model.MaxPeople,
            RecruitingMode = (int)model.RecruitingMode,
            ShowParticipants = model.ShowParticipants,
            OwnerId = user.Id,
            EventDate = model.EventDate,
            Location = model.Location,
            MapUrl = model.mapURL,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _activitiesContext.Activities.Add(activity);
        await _activitiesContext.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
    }

    [HttpGet("Detail/{id}")]
    [HttpGet("ActivityDetail/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> Detail(Guid id)
    {
        Activity activity = await _activitiesContext.Activities.FindAsync(id);
        ActivityDTO dto = new ActivityDTO()
        {
            Act = activity,
            Comments = await _ComSer.ShowCommentDTOs(id)
        };
        if (activity == null)
            return NotFound();

        return View("ActivityDetail", dto);
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KMITL_WebDev_MiniProject.Entites;
using KMITL_WebDev_MiniProject.Models;
using Microsoft.AspNetCore.Identity;
using KMITL_WebDev_MiniProject.DTO;
using MvcMovie.Migrations.ApplicationUserUtil;
using KMITL_WebDev_MiniProject.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

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
            Id = Guid.NewGuid(),
            Name = model.Name,
            Description = model.Description,
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

    [HttpGet("Detail/{id}")]
    [HttpGet("ActivityDetail/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> Detail(Guid id)
    {
        Activity activity = await _activitiesContext.Activities.FindAsync(id);
        if (activity == null)
            return NotFound();

        // look up owner name
        string ownerName = "";
        if (activity.OwnerId != Guid.Empty)
        {
            var owner = await _userManager.FindByIdAsync(activity.OwnerId.ToString());
            if (owner != null)
                ownerName = owner.RealUserName ?? owner.UserName;
        }

        ActivityDTO dto = new ActivityDTO()
        {
            Act = activity,
            Comments = await _ComSer.ShowCommentDTOs(id),
            OwnerName = ownerName
        };

        return View("ActivityDetail", dto);
    }
}

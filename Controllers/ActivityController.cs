using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using KMITL_WebDev_MiniProject.Entites;
using KMITL_WebDev_MiniProject.Models;
using Microsoft.AspNetCore.Identity;
using KMITL_WebDev_MiniProject.DTO;
using KMITL_WebDev_MiniProject.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace KMITL_WebDev_MiniProject.Controllers;

[Authorize]
[Route("Activity")]
public class ActivityController : Controller
{
    private const string DefaultOwnerImagePath = "image/UserProfile/guest_picture.jpg";

    private readonly ApplicationActivitiesDbContext _activitiesContext;
    private readonly UserManager<UserAccount> _userManager;
    private readonly CommentServices _commentService;
    private readonly FileUploadServcies _fileUploader;

    public ActivityController(
        ApplicationActivitiesDbContext activitiesContext,
        ApplicationUserUtilDbContext userUtilDbContext,
        UserManager<UserAccount> userManager,
        IWebHostEnvironment env)
    {
        _activitiesContext = activitiesContext;
        _userManager = userManager;
        _commentService = new CommentServices(userUtilDbContext, _userManager);
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
        ValidateRecruitingMode(model.RecruitingMode);

        if (!ModelState.IsValid)
            return View(model);

        var user = await GetCurrentUserAsync();
        if (user == null)
            return Forbid();

        var activity = new Activity
        {
            Id = Guid.NewGuid(),
            OwnerId = user.Id,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        ApplyModelToActivity(model, activity);

        _activitiesContext.Activities.Add(activity);
        await _activitiesContext.SaveChangesAsync();

        var hasUploadedImage = await UploadActivityImageIfProvidedAsync(model.ActivityImage, activity);
        if (hasUploadedImage)
        {
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

        var user = await GetCurrentUserAsync();
        if (user == null)
            return Challenge();

        if (!IsOwner(activity, user.Id))
            return Forbid();

        var model = new ActivityViewModel
        {
            Id = activity.Id,
            Name = activity.Name,
            Description = activity.Description,
            KeywordInput = activity.KeywordsText,
            MaxPeople = activity.MaxPeople,
            RecruitingMode = (RecruitingMode)activity.RecruitingMode,
            ShowParticipants = activity.ShowParticipants,
            EventDate = ToMinutePrecision(activity.EventDate),
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

        ValidateRecruitingMode(model.RecruitingMode);

        var activity = await _activitiesContext.Activities.FirstOrDefaultAsync(a => a.Id == id);
        if (activity == null)
            return NotFound();

        var user = await GetCurrentUserAsync();
        if (user == null)
            return Challenge();

        if (!IsOwner(activity, user.Id))
            return Forbid();

        if (!ModelState.IsValid)
            return View(model);

        ApplyModelToActivity(model, activity);
        await UploadActivityImageIfProvidedAsync(model.ActivityImage, activity);

        _activitiesContext.Activities.Update(activity);
        await _activitiesContext.SaveChangesAsync();

        return RedirectToAction(nameof(Detail), new { id = activity.Id });
    }

    [HttpGet("Detail/{id}")]
    [HttpGet("ActivityDetail/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> Detail(Guid id)
    {
        var activity = await LoadActivityForDetailAsync(id);
        if (activity == null)
            return NotFound();

        if (ShouldFinalizeRandomOnEventDay(activity))
        {
            await FinalizeRandomOnEventDayAsync(activity);

            activity = await LoadActivityForDetailAsync(id);

            if (activity == null)
                return NotFound();
        }

        var (ownerName, ownerImagePath) = await GetOwnerDisplayInfoAsync(activity.OwnerId);

        var currentUser = await GetCurrentUserAsync();
        var likeCount = await FindRelation(activity.Id);
        var isLike = currentUser != null && await IsLike(currentUser.Id, activity.Id);

        var dto = new ActivityDTO
        {
            Act = activity,
            Comments = await _commentService.ShowCommentDTOs(id),
            OwnerName = ownerName,
            OwnerImagePath = ownerImagePath,
            LikeCount = likeCount,
            IsLike = isLike,
            IsOwner = currentUser != null && IsOwner(activity, currentUser.Id),
            IsJoined = currentUser != null && activity.Participants.Any(p => p.Id == currentUser.Id)
        };

        return View("ActivityDetail", dto);
    }

    [HttpPost("Join/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Join(Guid id)
    {
        var user = await GetCurrentUserAsync();
        if (user == null)
            return Challenge();

        var activity = await LoadActivityWithUsersAsync(id);
        if (activity == null)
            return NotFound();

        if (IsOwner(activity, user.Id))
            return RedirectToDetail(id);

        if (TryFindParticipantRelation(activity, user.Id) != null)
            return RedirectToDetail(id);

        if (activity.EventDate <= DateTime.Now)
            return RedirectToDetail(id);

        if (IsJoinFullForCurrentMode(activity))
            return RedirectToDetail(id);

        activity.ActivityUsers.Add(new ActivityUser
        {
            ActivityId = activity.Id,
            UserId = user.Id,
            Role = ActivityUserRole.Participant
        });
        activity.UpdatedAt = DateTime.Now;
        await _activitiesContext.SaveChangesAsync();

        return RedirectToDetail(id);
    }

    [HttpPost("Close/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Close(Guid id)
    {
        var user = await GetCurrentUserAsync();
        if (user == null)
            return Challenge();

        var activity = await _activitiesContext.Activities.FirstOrDefaultAsync(a => a.Id == id);
        if (activity == null)
            return NotFound();

        if (!IsOwner(activity, user.Id))
            return Forbid();

        if (activity.EventDate > DateTime.Now)
        {
            // Mark as closed by moving event time to the past.
            activity.EventDate = DateTime.Now.AddSeconds(-1);
            activity.UpdatedAt = DateTime.Now;
            await _activitiesContext.SaveChangesAsync();
        }

        await FinalizeRandomOnEventDayAsync(activity);

        return RedirectToDetail(id);
    }

    [HttpPost("Unjoin/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unjoin(Guid id)
    {
        var user = await GetCurrentUserAsync();
        if (user == null)
            return Challenge();

        var activity = await LoadActivityWithUsersAsync(id);
        if (activity == null)
            return NotFound();

        if (IsOwner(activity, user.Id))
            return RedirectToDetail(id);

        var participantRelation = TryFindParticipantRelation(activity, user.Id);

        if (participantRelation == null)
            return RedirectToDetail(id);

        activity.ActivityUsers.Remove(participantRelation);
        activity.UpdatedAt = DateTime.Now;
        await _activitiesContext.SaveChangesAsync();

        return RedirectToDetail(id);
    }

    [HttpPost("ToggleLock/{id}/{userId}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleLock(Guid id, Guid userId)
    {
        var owner = await GetCurrentUserAsync();
        if (owner == null)
            return Challenge();

        var activity = await LoadActivityWithUsersAsync(id);
        if (activity == null)
            return NotFound();

        if (!IsOwner(activity, owner.Id))
            return Forbid();

        if (activity.RecruitingMode != (int)RecruitingMode.OwnerSelect)
            return RedirectToDetail(id);

        if (activity.EventDate <= DateTime.Now)
            return RedirectToDetail(id);

        var relation = TryFindParticipantRelation(activity, userId);

        if (relation == null)
            return RedirectToDetail(id);

        var newRole = relation.Role == ActivityUserRole.Locked
            ? ActivityUserRole.Participant
            : ActivityUserRole.Locked;

        // Role is part of composite key {ActivityId, UserId, Role}, so replace the row instead of mutating key.
        activity.ActivityUsers.Remove(relation);
        activity.ActivityUsers.Add(new ActivityUser
        {
            ActivityId = id,
            UserId = userId,
            Role = newRole
        });

        activity.UpdatedAt = DateTime.Now;
        await _activitiesContext.SaveChangesAsync();

        return RedirectToDetail(id);
    }

    private static DateTime ToMinutePrecision(DateTime value)
    {
        return new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, 0, value.Kind);
    }

    private async Task FinalizeRandomOnEventDayAsync(Activity activity)
    {
        if (activity.RecruitingMode != (int)RecruitingMode.RandomOnEventDay)
            return;

        if (activity.EventDate.AddHours(-1) > DateTime.Now)
            return;

        var guestSlots = Math.Max(0, activity.MaxPeople - 1);
        var participants = activity.ActivityUsers
            .Where(au => au.Role == ActivityUserRole.Participant)
            .ToList();

        if (participants.Count <= guestSlots)
            return;

        var winnerIds = participants
            .OrderBy(_ => Guid.NewGuid())
            .Take(guestSlots)
            .Select(au => au.UserId)
            .ToHashSet();

        var losers = participants.Where(au => !winnerIds.Contains(au.UserId)).ToList();
        foreach (var loser in losers)
        {
            activity.ActivityUsers.Remove(loser);
        }

        activity.UpdatedAt = DateTime.Now;
        await _activitiesContext.SaveChangesAsync();
    }

    [HttpPost]
    [Authorize]
    public async Task UpdateRelation([FromBody] ActIDDTO Data)
    {
        var user = await GetCurrentUserAsync();
        if (user == null)
            return;

        var relation = await _activitiesContext.Relations
            .FirstOrDefaultAsync(rel => rel.UserID == user.Id && rel.ActID == Data.ActID);

        if (relation == null)
        {
            relation = new ActivityRelation
            {
                UserID = user.Id,
                ActID = Data.ActID,
                Relation = 1
            };
            await _activitiesContext.Relations.AddAsync(relation);
        }
        else
        {
            relation.Relation ^= 0b1;
            _activitiesContext.Relations.Entry(relation);
        }

        await _activitiesContext.SaveChangesAsync();
    }

    [HttpGet("FindRelation/{ActID}")]
    [Authorize]
    public async Task<int> FindRelation(Guid ActID)
    {
        return await GetLikeCountAsync(ActID);
    }

    [HttpGet("IsLike/{UserID}&{ActID}")]
    [Authorize]
    public async Task<bool> IsLike(Guid UserID, Guid ActID)
    {
        return await IsLikedByUserAsync(UserID, ActID);
    }

    private async Task<UserAccount?> GetCurrentUserAsync()
    {
        return await _userManager.GetUserAsync(User);
    }

    private async Task<Activity?> LoadActivityForDetailAsync(Guid id)
    {
        return await _activitiesContext.Activities
            .Where(a => a.Id == id)
            .Include(a => a.ActivityUsers)
                .ThenInclude(au => au.User)
            .FirstOrDefaultAsync();
    }

    private async Task<Activity?> LoadActivityWithUsersAsync(Guid id)
    {
        return await _activitiesContext.Activities
            .Include(a => a.ActivityUsers)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    private static bool IsOwner(Activity activity, Guid userId)
    {
        return activity.OwnerId == userId;
    }

    private static ActivityUser? TryFindParticipantRelation(Activity activity, Guid userId)
    {
        return activity.ActivityUsers.FirstOrDefault(au =>
            au.UserId == userId && (au.Role == ActivityUserRole.Participant || au.Role == ActivityUserRole.Locked));
    }

    private static bool ShouldFinalizeRandomOnEventDay(Activity activity)
    {
        return activity.RecruitingMode == (int)RecruitingMode.RandomOnEventDay
            && activity.EventDate.AddHours(-1) <= DateTime.Now;
    }

    private async Task<(string OwnerName, string OwnerImagePath)> GetOwnerDisplayInfoAsync(Guid ownerId)
    {
        if (ownerId == Guid.Empty)
            return (string.Empty, DefaultOwnerImagePath);

        var owner = await _userManager.FindByIdAsync(ownerId.ToString());
        if (owner == null)
            return (string.Empty, DefaultOwnerImagePath);

        var ownerName = owner.RealUserName ?? owner.UserName ?? string.Empty;
        var ownerImagePath = string.IsNullOrWhiteSpace(owner.ImagePath) ? DefaultOwnerImagePath : owner.ImagePath;
        return (ownerName, ownerImagePath);
    }

    private void ValidateRecruitingMode(RecruitingMode mode)
    {
        if (!Enum.IsDefined(typeof(RecruitingMode), mode))
            ModelState.AddModelError(nameof(ActivityViewModel.RecruitingMode), "Invalid recruiting mode.");
    }

    private static string NormalizeKeywords(string? input)
    {
        return string.Join(", ", (input ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(k => k.Trim())
            .Where(k => !string.IsNullOrWhiteSpace(k))
            .Distinct(StringComparer.OrdinalIgnoreCase));
    }

    private void ApplyModelToActivity(ActivityViewModel model, Activity activity)
    {
        activity.Name = model.Name;
        activity.Description = model.Description;
        activity.KeywordsText = NormalizeKeywords(model.KeywordInput);
        activity.MaxPeople = model.MaxPeople;
        activity.RecruitingMode = (int)model.RecruitingMode;
        activity.ShowParticipants = model.ShowParticipants;
        activity.EventDate = ToMinutePrecision(model.EventDate);
        activity.Location = model.Location;
        activity.MapUrl = model.MapUrl;
        activity.UpdatedAt = DateTime.Now;
    }

    private async Task<bool> UploadActivityImageIfProvidedAsync(IFormFile? file, Activity activity)
    {
        if (file == null || !_fileUploader.FileIsExist(file))
            return false;

        await _fileUploader.Upload(file, activity.Id.ToString());
        activity.ImageUrl = Path.Combine("image", "UserProfile", activity.Id + _fileUploader.LastExt);
        return true;
    }

    private bool IsJoinFullForCurrentMode(Activity activity)
    {
        var normalParticipantCount = activity.ActivityUsers.Count(au => au.Role == ActivityUserRole.Participant);
        var lockedParticipantCount = activity.ActivityUsers.Count(au => au.Role == ActivityUserRole.Locked);
        var attendeeCountIncludingOwner = normalParticipantCount + lockedParticipantCount + 1;
        var guestSlots = Math.Max(0, activity.MaxPeople - 1);

        var mode = (RecruitingMode)activity.RecruitingMode;
        if (mode == RecruitingMode.FirstComeFirstServe)
            return attendeeCountIncludingOwner >= activity.MaxPeople;

        if (mode == RecruitingMode.OwnerSelect)
        {
            var publicSlots = Math.Max(0, guestSlots - lockedParticipantCount);
            return normalParticipantCount >= publicSlots;
        }

        return false;
    }

    private async Task<int> GetLikeCountAsync(Guid activityId)
    {
        return await _activitiesContext.Relations
            .Where(rel => rel.ActID == activityId && rel.Relation == 1)
            .CountAsync();
    }

    private async Task<bool> IsLikedByUserAsync(Guid userId, Guid activityId)
    {
        return await _activitiesContext.Relations
            .AnyAsync(rel => rel.UserID == userId && rel.ActID == activityId && rel.Relation == 1);
    }

    private IActionResult RedirectToDetail(Guid id)
    {
        return RedirectToAction(nameof(Detail), new { id });
    }
}
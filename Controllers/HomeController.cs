// using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using KMITL_WebDev_MiniProject.Models;
using KMITL_WebDev_MiniProject.Entites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using KMITL_WebDev_MiniProject.Entites;
using KMITL_WebDev_MiniProject.Models;

// alias to avoid conflict with System.Diagnostics.Activity
using ActivityEntity = KMITL_WebDev_MiniProject.Entites.Activity;

namespace KMITL_WebDev_MiniProject.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationActivitiesDbContext _activitiesContext;
    private readonly UserManager<UserAccount> _userManager;

    public HomeController(ApplicationActivitiesDbContext activitiesContext, UserManager<UserAccount> userManager)
    {
        _activitiesContext = activitiesContext;
        _userManager = userManager;
    }

    [HttpGet]
    [Authorize]
    [Route("Home")]	
    [Route("Home/Index")]	
    public async Task<IActionResult> Index()
    {
        var activities = await _activitiesContext.Activities.ToListAsync();
        var previews = new List<ActivityPreviewViewModel>(activities.Count);
        foreach (var act in activities)
        {
            var ownerName = string.Empty;
            if (act.OwnerId != Guid.Empty)
            {
                var owner = await _userManager.FindByIdAsync(act.OwnerId.ToString());
                if (owner != null)
                    ownerName = owner.RealUserName ?? owner.UserName;
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
    public IActionResult ActivityDetail()
    {
        return View();
    }

    public IActionResult Profile()
    {
        return View();
    }

    // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    // [AllowAnonymous]
    // public IActionResult Error()
    // {
    //     return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    // }
}
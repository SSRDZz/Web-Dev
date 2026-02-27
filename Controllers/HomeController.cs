using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using KMITL_WebDev_MiniProject.Models;
using KMITL_WebDev_MiniProject.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

// alias to avoid conflict with System.Diagnostics.Activity
using ActivityEntity = KMITL_WebDev_MiniProject.Entites.Activity;

namespace KMITL_WebDev_MiniProject.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationActivitiesDbContext _activitiesContext;

    public HomeController(ApplicationActivitiesDbContext activitiesContext)
    {
        _activitiesContext = activitiesContext;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Index()
    {
        var activities = await _activitiesContext.Activities.ToListAsync();
        return View(activities);
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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [AllowAnonymous]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

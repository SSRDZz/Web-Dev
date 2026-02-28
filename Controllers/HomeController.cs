using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using KMITL_WebDev_MiniProject.Models;
using Microsoft.AspNetCore.Authorization;

namespace KMITL_WebDev_MiniProject.Controllers;

public class HomeController : Controller
{
    [HttpGet]
    [Authorize]
    public IActionResult Index()
    {
        return View();
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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [AllowAnonymous]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

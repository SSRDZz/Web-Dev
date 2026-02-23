using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using KMITL_WebDev_MiniProject.Models;

namespace KMITL_WebDev_MiniProject.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        if(User.Identity != null && !User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Auth");
        
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult ActivityDetail()
    {
        if(User.Identity != null && !User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Auth");

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

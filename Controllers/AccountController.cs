using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using KMITL_WebDev_MiniProject.Models;

namespace KMITL_WebDev_MiniProject.Controllers;

public class AccountController : Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {
        Console.WriteLine(model.Email);
        Console.WriteLine(model.Password);
        Console.WriteLine(model.RememberMe);
        return RedirectToAction("Login");
    }

    public IActionResult About()
    {
        return View();
    }

	public int GetNum(int num)
	{
		int sum = 0;
		for(int i = 1; i <= num; i++)
		{
			sum += i;
		}
		return sum;
	}

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

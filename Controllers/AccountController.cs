using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using KMITL_WebDev_MiniProject.Models;
using Microsoft.AspNetCore.Identity;

namespace KMITL_WebDev_MiniProject.Controllers
{
    public class AccountController : Controller
    {
        // private readonly SignInManager   
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if(ModelState.IsValid)
            {
                // var result = await SignInManager
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult About()
        {
            return View();
        }

        [HttpGet]
        public int GetNum(int num)
        {
            return (num)*(num+1)/2;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using KMITL_WebDev_MiniProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using KMITL_WebDev_MiniProject.Entites;
using KMITL_WebDev_MiniProject.Data;
using Microsoft.EntityFrameworkCore;

namespace KMITL_WebDev_MiniProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<UserAccount> _signInManager;
        private readonly UserManager<UserAccount> _userManager;

        public AccountController(SignInManager<UserAccount> signInManager, UserManager<UserAccount> userManager)
        {
            this._signInManager = signInManager;
            this._userManager = userManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            if(User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if(ModelState.IsValid)
            {
                UserAccount account = new UserAccount()
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailConfirmed = true
                };

                try
                {
                    this._userManager.CreateAsync(account, model.Password);

                    ModelState.Clear();
                    ViewBag.Message = $"{account.FirstName} {account.LastName} registered successfully. Please Login.";    
                }
                catch
                {
                    ModelState.AddModelError("", "Please enter unique Email or Password");
                    return View(model);
                }
                // Console.WriteLine("Register complete");
                return RedirectToAction("Login", "Account");
            }
            return View(model);
        }


        [HttpGet]
        public IActionResult Login()
        {
            if(User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            Console.WriteLine(model.Email + " " + model.Password + " " + model.RememberMe.ToString());
            if(ModelState.IsValid)
            {
                var result = await this._signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if(result.Succeeded) return RedirectToAction("Index", "Home");
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await this._signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
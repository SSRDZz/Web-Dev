using Microsoft.AspNetCore.Mvc;
using KMITL_WebDev_MiniProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using KMITL_WebDev_MiniProject.Entites;
using KMITL_WebDev_MiniProject.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using SQLitePCL;
using Microsoft.EntityFrameworkCore.Query;

namespace KMITL_WebDev_MiniProject.Controllers
{
	public class AccountController : Controller
	{
		private readonly SignInManager<UserAccount> _signInManager;
		private readonly UserManager<UserAccount> _userManager;
		private readonly ApplicationDbContext _dbContext;

		public AccountController(
			SignInManager<UserAccount> signInManager, 
			UserManager<UserAccount> userManager,
			ApplicationDbContext dbContext)
		{
			this._signInManager = signInManager;
			this._userManager = userManager;
			this._dbContext = dbContext;
		}

		[HttpGet]
		public IActionResult Register()
		{
			if(User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterViewModel model)
		{
			if(ModelState.IsValid)
			{
				UserAccount account = new UserAccount()
				{
					UserName = model.Email,
					Email = model.Email,
					FirstName = model.FirstName,
					LastName = model.LastName,
					EmailConfirmed = false
				};

				var result = await this._userManager.CreateAsync(account, model.Password);
				if(!result.Succeeded)
				{
					ModelState.AddModelError("", "Please enter unique Email or Password");
					return View(model);
				}
				ModelState.Clear();
				ViewBag.Message = $"{account.FirstName} {account.LastName} registered successfully. Please Login.";    
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

		[HttpGet]
		public async Task<IActionResult> Profile()
		{
			if(!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Account");
			string curUserIDString = this._userManager.GetUserId(User).ToString();
			Console.WriteLine(curUserIDString);
			var account = this._dbContext.Users.Where(r => r.Id.ToString() == curUserIDString).ToList()[0];
			ProfileViewModel profileObj = new ProfileViewModel()
			{
				FirstName = account.FirstName,
				LastName = account.LastName
			};
			return View(profileObj);
		}

		[HttpGet]
		public async Task<IActionResult> ProfileEdit()
		{
			if(!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Account");
			string curUserIDString = this._userManager.GetUserId(User).ToString();
			Console.WriteLine(curUserIDString);
			var account = this._dbContext.Users.Where(r => r.Id.ToString() == curUserIDString).ToList()[0];
			ProfileViewModel profileObj = new ProfileViewModel()
			{
				FirstName = account.FirstName,
				LastName = account.LastName
			};
			return View(profileObj);
		}
	}
}
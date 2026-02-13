using Microsoft.AspNetCore.Mvc;
using KMITL_WebDev_MiniProject.Models;
using Microsoft.AspNetCore.Identity;
using KMITL_WebDev_MiniProject.Entites;
using KMITL_WebDev_MiniProject.Data;
using KMITL_WebDev_MiniProject.Services;

namespace KMITL_WebDev_MiniProject.Controllers
{
	public class AccountController : Controller
	{
		private readonly SignInManager<UserAccount> _signInManager;
		private readonly UserManager<UserAccount> _userManager;
		private readonly ApplicationDbContext _dbContext;
		private readonly UserServices _userServices;

		public AccountController(
			SignInManager<UserAccount> signInManager, 
			UserManager<UserAccount> userManager,
			ApplicationDbContext dbContext)
		{
			_signInManager = signInManager;
			_userManager = userManager;
			_dbContext = dbContext;
			_userServices = new UserServices(_userManager, _dbContext);
		}

		[HttpGet]
		public IActionResult Register()
		{
			if(User.Identity.IsAuthenticated) 
				return RedirectToAction("Index", "Home");
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

				var result = await _userManager.CreateAsync(account, model.Password);
				if(!result.Succeeded)
				{
					ModelState.AddModelError("", "Please enter unique Email or Password");
					return View(model);
				}
				ModelState.Clear();
				// ViewBag.Message = $"{account.FirstName} {account.LastName} registered successfully. Please Login.";
				var res = await _signInManager.PasswordSignInAsync(account.Email, model.Password, false, lockoutOnFailure: false);
				if(!res.Succeeded) return RedirectToAction("Login", "Account");
				return RedirectToAction("Index", "home");
			}
			return View(model);
		}


		[HttpGet]
		public IActionResult Login()
		{
			if(User.Identity.IsAuthenticated) 
				return RedirectToAction("Index", "Home");
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			if(ModelState.IsValid)
			{
				var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
				if(result.Succeeded) 
					return RedirectToAction("Index", "Home");
				ModelState.AddModelError(string.Empty, "Invalid login attempt.");
			}
			return View(model);
		}
	
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Login", "Account");
		}

		[HttpGet]
		public async Task<IActionResult> Profile()
		{
			if(!User.Identity.IsAuthenticated) 
				return RedirectToAction("Login", "Account");
			return View(_userServices.getProfileViewModelByUser(User));
		}

		[HttpGet]
		public async Task<IActionResult> ProfileEdit()
		{
			if(!User.Identity.IsAuthenticated) 
				return RedirectToAction("Login", "Account");
			return View(_userServices.getProfileViewModelByUser(User));
		}

		[HttpPost]
		public async Task<IActionResult> ProfileUpdate(ProfileViewModel model)
		{
			if(!User.Identity.IsAuthenticated) 
				return RedirectToAction("Login", "Account");

			if(ModelState.IsValid)
			{
				var user = await _userManager.GetUserAsync(User);
				if(user != null) 
				{
					user.FirstName = model.FirstName;
					user.LastName  = model.LastName;
					var res = await _userManager.UpdateAsync(user);
					if(!res.Succeeded)
					{
						return View(model);
					}
					return RedirectToAction("Profile", "Account");
				}
			}
			return View(model);
		}
	}
}
using Microsoft.AspNetCore.Mvc;
using KMITL_WebDev_MiniProject.Models;
using Microsoft.AspNetCore.Identity;
using KMITL_WebDev_MiniProject.Entites;
using KMITL_WebDev_MiniProject.Data;
using KMITL_WebDev_MiniProject.Services;

namespace KMITL_WebDev_MiniProject.Controllers
{
	public class AuthController : Controller
	{
		private readonly SignInManager<UserAccount> _signInManager;
		private readonly UserManager<UserAccount> _userManager;
		private readonly ApplicationDbContext _dbContext;
		private readonly UserServices _userServices;

		public AuthController(
			SignInManager<UserAccount> signInManager, 
			UserManager<UserAccount> userManager,
			ApplicationDbContext dbContext,
			IWebHostEnvironment env)
		{
			_signInManager = signInManager;
			_userManager = userManager;
			_dbContext = dbContext;
			_userServices = new UserServices(_userManager, _dbContext, env);
		}

		[HttpGet]
		public IActionResult Register()
		{
			if(User.Identity.IsAuthenticated)
				return RedirectToAction("Index", "Home");
			return View();
		}

		[HttpPost]
		public IActionResult RegisterInsertProfile(RegisterViewModel model)
		{
			if(User.Identity.IsAuthenticated)
				return RedirectToAction("Index", "Home");

			TempData["MyModel"] = Newtonsoft.Json.JsonConvert.SerializeObject(model);
			
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> RegisterInsertProfileLogic(IFormFile? profilePicture)
		{
			if(User.Identity.IsAuthenticated)
				return RedirectToAction("Index", "Home");

			RegisterViewModel model = Newtonsoft.Json.JsonConvert.DeserializeObject<RegisterViewModel>(TempData["MyModel"] as string);
			if(model.Email == null || model.Password == null)
				return RedirectToAction("Register");

			string imgBase64 = await _userServices.ImageFileToBase64(profilePicture);
			model.ImageURL = (string.IsNullOrEmpty(imgBase64)) ? _userServices.guestImageURL : imgBase64;

			// this move was better cause no large data transfer between request but now can't do it, skill issue!
			// TempData["FinalModel"] = Newtonsoft.Json.JsonConvert.SerializeObject(model);
			// return RedirectToAction("Register", null);

			return await Register(model);
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterViewModel model)
		{
			// code for upper better move
			// RegisterViewModel finalModel = Newtonsoft.Json.JsonConvert.DeserializeObject<RegisterViewModel>(TempData["FinalModel"] as string);
			// bool stateA = (finalModel != null) && (finalModel.Email == null || finalModel.Password == null);

			if(!ModelState.IsValid)
				return RedirectToAction("Register");
			
			UserAccount account = new UserAccount()
			{
				UserName = model.Email,
				RealUserName = model.UserName,
				Email = model.Email,
				FirstName = model.FirstName,
				LastName = model.LastName,
				PhoneNumber = model.PhoneNumber,
				DateOfBirth = model.DateOfBirth,
				ImageURL = model.ImageURL,
				EmailConfirmed = false
			};

			var result = await _userManager.CreateAsync(account, model.Password);
			if(!result.Succeeded)
			{
				ModelState.AddModelError("", "Please enter unique Email or Password");
				return View(model);
			}
			ModelState.Clear();
			
			var res = await _signInManager.PasswordSignInAsync(account.Email, model.Password, false, lockoutOnFailure: false);
			if(!res.Succeeded) 
				return RedirectToAction("Login");

			return RedirectToAction("Index", "Home");
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
			return RedirectToAction("Login");
		}
	}
}
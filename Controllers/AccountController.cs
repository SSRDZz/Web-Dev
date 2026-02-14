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

		[HttpGet]
		public IActionResult RegisterInsertProfile(RegisterViewModel model)
		{
			if(User.Identity.IsAuthenticated)
				return RedirectToAction("Index", "Home");

			TempData["MyModel"] = Newtonsoft.Json.JsonConvert.SerializeObject(model);
			
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> RegisterInsertProfile(IFormFile? profilePicture)
		{
			if(User.Identity.IsAuthenticated)
				return RedirectToAction("Index", "Home");

			RegisterViewModel model = Newtonsoft.Json.JsonConvert.DeserializeObject<RegisterViewModel>(TempData["MyModel"] as string);
			if(model.Email == null || model.Password == null)
				return Register();

			if(profilePicture == null)
			{
				model.ImageURL = "";
				return Register(model).Result;
			}

			if(profilePicture != null && profilePicture.Length > 0)
			{
				using (var memoryStream = new MemoryStream())
				{
					await profilePicture.CopyToAsync(memoryStream);
					model.ImageURL = Convert.ToBase64String(memoryStream.ToArray());
				}
			}

			return Register(model).Result;
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
					return Login();

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
			return Login();
		}

		[HttpGet]
		public async Task<IActionResult> Profile()
		{
			if(!User.Identity.IsAuthenticated) 
				return Login();
				
			return View(_userServices.getProfileViewModelByUser(User));
		}

		[HttpGet]
		public async Task<IActionResult> ProfileEdit()
		{
			if(!User.Identity.IsAuthenticated) 
				return Login();
				
			return View(_userServices.getProfileViewModelByUser(User));
		}

		[HttpPost]
		public async Task<IActionResult> ProfileUpdate(ProfileViewModel model, IFormFile? newPicture)
		{
			if(!User.Identity.IsAuthenticated) 
				return Login();

			if(!ModelState.IsValid)
				return Profile().Result;

			var user = await _userManager.GetUserAsync(User);

			if(user == null)
				return  NotFound();

			if(newPicture != null && newPicture.Length > 0)
			{
				using (var memoryStream = new MemoryStream())
				{
					await newPicture.CopyToAsync(memoryStream);
					user.ImageURL = Convert.ToBase64String(memoryStream.ToArray());
				}
			}
			
			user.FirstName = model.FirstName;
			user.LastName  = model.LastName;

			var res = await _userManager.UpdateAsync(user);

			if(!res.Succeeded)
			{
				foreach (var err in res.Errors)
					Console.WriteLine(err.Code + " : " + err.Description);
				Console.WriteLine("Failed");
				return NotFound();
			}

			return Profile().Result;
		}
	}
}
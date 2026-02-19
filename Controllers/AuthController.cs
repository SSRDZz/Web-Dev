using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using KMITL_WebDev_MiniProject.Models;
using KMITL_WebDev_MiniProject.Entites;
using KMITL_WebDev_MiniProject.Services;

namespace KMITL_WebDev_MiniProject.Controllers;
public class AuthController(SignInManager<UserAccount> signInManager, UserManager<UserAccount> userManager, IWebHostEnvironment env) : Controller
{
	private SignInManager<UserAccount> _signInManager {get; init;} = signInManager;
	private UserManager<UserAccount> _userManager {get; init;} = userManager; 
	private UserServices _userServices {get; init;} = new UserServices(userManager, env);

	[HttpGet]
	public IActionResult Register()
	{
		if(User.Identity == null || User.Identity.IsAuthenticated)
			return RedirectToAction("Index", "Home");

		return View();
	}

	[HttpPost]
	public IActionResult RegisterInsertProfile(RegisterViewModel model)
	{
		if(User.Identity == null || User.Identity.IsAuthenticated)
			return RedirectToAction("Index", "Home");

		TempData["MyModel"] = Newtonsoft.Json.JsonConvert.SerializeObject(model);
		
		return View(model);
	}

	[HttpPost]
	public async Task<IActionResult> RegisterInsertProfileLogic(IFormFile? profilePicture)
	{
		if(User.Identity == null || User.Identity.IsAuthenticated)
			return RedirectToAction("Index", "Home");

		RegisterViewModel? model = Newtonsoft.Json.JsonConvert.DeserializeObject<RegisterViewModel>(TempData["MyModel"] as string);
		if(model == null || model.Email == null || model.Password == null)
			return RedirectToAction("Register");

		string? imgBase64 = await _userServices.ImageFileToBase64(profilePicture);
		model.ImageURL = string.IsNullOrEmpty(imgBase64) ? _userServices.guestImageURL : imgBase64;

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
		
		UserAccount account = _userServices.RegisterViewModelToAccount(model);

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
		if(User.Identity == null || User.Identity.IsAuthenticated) 
			return RedirectToAction("Index", "Home");
		
		return View();
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Login(LoginViewModel model)
	{
		if(!ModelState.IsValid)
			return View(model);
		
		var result = await _signInManager.PasswordSignInAsync(
			model.Email, 
			model.Password, 
			model.RememberMe, 
			lockoutOnFailure: false);
			
		if(!result.Succeeded)
		{
			ViewBag.login = "0";
			return View(model);
		}

		ViewBag.login = "1";
		return RedirectToAction("Index", "Home");
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Logout()
	{
		await _signInManager.SignOutAsync();
		return RedirectToAction("Login");
	}
}
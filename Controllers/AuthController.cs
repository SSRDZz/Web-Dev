using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using KMITL_WebDev_MiniProject.Models;
using KMITL_WebDev_MiniProject.Entites;
using KMITL_WebDev_MiniProject.Services;

namespace KMITL_WebDev_MiniProject.Controllers;
public class AuthController(SignInManager<UserAccount> signInManager, UserManager<UserAccount> userManager, IWebHostEnvironment env, ApplicationReputationsDbContext RepDbContext) : Controller
{
	private SignInManager<UserAccount> _signInManager {get; init;} = signInManager;
	private UserManager<UserAccount> _userManager {get; init;} = userManager; 
	private UserServices _userServices {get; init;} = new UserServices(userManager, env, RepDbContext);

	[HttpGet]
	public IActionResult Register()
	{
		if(User.Identity == null || User.Identity.IsAuthenticated)
			return RedirectToAction("Index", "Home");
			
		return View();
	}

	[HttpPost]
	public async Task<IActionResult> Register(RegisterViewModel model)
	{
		if(User.Identity == null || User.Identity.IsAuthenticated)
			return RedirectToAction("Index","Home");

		if(!ModelState.IsValid)
			return RedirectToAction("Register");
		
		UserAccount account = _userServices.RegisterViewModelToAccount(model);

		if(await _userServices.IsRealNameExist(account.RealUserName))
		{
			ViewBag.errors = new IdentityError[] { new IdentityError {Description = "Username is already exist"}};
			return View(model);
		}

		IdentityResult result = await _userManager.CreateAsync(account, model.Password);
		if(!result.Succeeded)
		{
			ViewBag.register = "0";
			ViewBag.errors = result.Errors.ToArray();
			return View(model);
		}
		ModelState.Clear();

		TempData["Email"] = model.Email;

		ViewBag.register = "1";
		return RedirectToAction("RegisterInsertProfile");
	}


	[HttpGet]
	public async Task<IActionResult> RegisterInsertProfile()
	{
		if(User.Identity != null && User.Identity.IsAuthenticated)
			return RedirectToAction("Login");
	
		string Email = TempData["Email"] as string;
		UserAccount user = await _userManager.FindByNameAsync(Email);

		if(user.ImageURL != _userServices.guestImageURL)
			return RedirectToAction("Index", "Home");

		await _signInManager.SignInAsync(user, false);

		return View();
	}

	[HttpPost]
	public async Task<IActionResult> RegisterInsertProfile(IFormFile? profilePicture)
	{
		if(User.Identity == null || User.Identity.IsAuthenticated)
			return RedirectToAction("Login");

		UserAccount user = await _userManager.GetUserAsync(User);

		if(user.ImageURL != _userServices.guestImageURL)
			return RedirectToAction("Index", "Home");

		string? imgBase64 = await _userServices.ImageFileToBase64(profilePicture);
		user.ImageURL = !string.IsNullOrEmpty(imgBase64) ?  imgBase64 : _userServices.guestImageURL;

		IdentityResult res = await _userManager.UpdateAsync(user);

		if(!res.Succeeded)
			return RedirectToAction("Register");

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
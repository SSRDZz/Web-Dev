using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using KMITL_WebDev_MiniProject.Models;
using KMITL_WebDev_MiniProject.Entites;
using KMITL_WebDev_MiniProject.Services;
using Microsoft.AspNetCore.Authorization;

namespace KMITL_WebDev_MiniProject.Controllers;
public class AuthController(SignInManager<UserAccount> signInManager, UserManager<UserAccount> userManager, IWebHostEnvironment env, ApplicationReputationsDbContext RepDbContext) : Controller
{
	private SignInManager<UserAccount> SignInMang {get; init;} = signInManager;
	private UserManager<UserAccount> UserMang {get; init;} = userManager; 
	private UserServices UserServ {get; init;} = new UserServices(userManager, env, RepDbContext);
	private FileUploadServcies FUS {get; init;} = new FileUploadServcies(env);

	[HttpGet]
	[AllowAnonymous]
	public IActionResult Register()
	{
		if(User.Identity != null && User.Identity.IsAuthenticated)
			return RedirectToAction("Index", "Home");
			
		return View();
	}

	[HttpPost]
	[AllowAnonymous]
	public async Task<IActionResult> Register(RegisterViewModel model)
	{
		if(User.Identity != null && User.Identity.IsAuthenticated)
			return RedirectToAction("Index","Home");

		if(!ModelState.IsValid)
			return RedirectToAction("Register");
		
		UserAccount account = UserServ.RegisterViewModelToAccount(model);
		account.ImagePath = UserServ.GuestImagePath;

		if(await UserServ.IsRealNameExist(account.RealUserName))
		{
			ViewBag.errors = new IdentityError[] { new IdentityError {Description = "Username is already exist"}};
			return View(model);
		}

		IdentityResult result = await UserMang.CreateAsync(account, model.Password);
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
	[AllowAnonymous]
	public async Task<IActionResult> RegisterInsertProfile()
	{
		if(User.Identity != null && User.Identity.IsAuthenticated)
			return RedirectToAction("Login");
	
		return View();
	}

	[HttpPost]
	[AllowAnonymous]
	public async Task<IActionResult> RegisterInsertProfile(IFormFile? profilePicture)
	{
		if(User.Identity != null && User.Identity.IsAuthenticated)
			return RedirectToAction("Login");

		string Email = TempData["Email"] as string;
		UserAccount user = await UserMang.FindByNameAsync(Email);
		await SignInMang.SignInAsync(user, false);

		await FUS.Upload(profilePicture, user.Id.ToString());

		// string? imgBase64 = await UserServ.ImageFileToBase64(profilePicture);
		// user.ImageURL = !string.IsNullOrEmpty(imgBase64) ?  imgBase64 : UserServ.guestImageURL;

		IdentityResult res = await UserMang.UpdateAsync(user);

		if(!res.Succeeded)
			return RedirectToAction("Register");

		return RedirectToAction("Index", "Home");
	}

	

	[HttpGet]
	[AllowAnonymous]
	public IActionResult Login()
	{
		if(User.Identity != null && User.Identity.IsAuthenticated) 
			return RedirectToAction("Index", "Home");
		
		return View();
	}

	[HttpPost]
	[AllowAnonymous]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Login(LoginViewModel model)
	{
		if(!ModelState.IsValid)
			return View(model);
		
		var result = await SignInMang.PasswordSignInAsync(
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
		await SignInMang.SignOutAsync();
		return RedirectToAction("Login");
	}
}
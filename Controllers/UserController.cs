using KMITL_WebDev_MiniProject.Entites;
using KMITL_WebDev_MiniProject.Models;
using KMITL_WebDev_MiniProject.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KMITL_WebDev_MiniProject.Controllers;
public class UserController(UserManager<UserAccount> userManager, IWebHostEnvironment env) : Controller
{
	private UserManager<UserAccount> _userManager {get; init;} = userManager;
	private UserServices _userServices {get; init;} = new UserServices(userManager, env);

	[HttpGet]
	public async Task<IActionResult> Profile()
	{
		if(User.Identity == null || !User.Identity.IsAuthenticated) 
			return RedirectToAction("Login", "Auth");
			
		return View(await _userServices.getProfileViewModelByUser(User));
	}

	[HttpGet]
	public async Task<IActionResult> ProfileEdit()
	{
		if(User.Identity == null || !User.Identity.IsAuthenticated) 
			return RedirectToAction("Login", "Auth");
			
		return View(await _userServices.getProfileViewModelByUser(User));
	}

	[HttpPost]
	public async Task<IActionResult> ProfileUpdate(ProfileViewModel model, IFormFile? newPicture)
	{
		if(User.Identity == null || !User.Identity.IsAuthenticated) 
			return RedirectToAction("Login", "Auth");
		
		if(!ModelState.IsValid)
			return RedirectToAction("Profile");

		UserAccount user = await _userManager.GetUserAsync(User);

		if(user == null)
			return NotFound();

		string imgBase64 = await _userServices.ImageFileToBase64(newPicture);
		user.ImageURL = !string.IsNullOrEmpty(imgBase64) ? imgBase64 : user.ImageURL;

		user.FirstName = model.FirstName;
		user.LastName  = model.LastName;

		IdentityResult res = await _userManager.UpdateAsync(user);

		if(!res.Succeeded)
		{
			foreach (var err in res.Errors)
				Console.WriteLine(err.Code + " : " + err.Description);
			Console.WriteLine("Update Failed");
			return NotFound();
		}

		return RedirectToAction("Profile");
	}

	[HttpGet]
	public async Task<IActionResult> ProfileOther(string Id)
	{
		if(!User.Identity.IsAuthenticated) 
			return RedirectToAction("Login", "Auth");

		UserAccount user = await _userManager.FindByIdAsync(Id);
		if(user == null)
			return RedirectToAction("Index", "Home");

		return View(_userServices.ToProfileViewModel(user));
	}
}
using KMITL_WebDev_MiniProject.DTO;
using KMITL_WebDev_MiniProject.Entites;
using KMITL_WebDev_MiniProject.Models;
using KMITL_WebDev_MiniProject.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KMITL_WebDev_MiniProject.Controllers;
public class UserController(UserManager<UserAccount> userManager, IWebHostEnvironment env, ApplicationReputationsDbContext RepDbContext) : Controller
{
	private UserManager<UserAccount> _userManager {get; init;} = userManager;
	private UserServices _userServices {get; init;} = new UserServices(userManager, env, RepDbContext);
	private ApplicationReputationsDbContext RepDbContext {get; init;} = RepDbContext;

	[HttpGet]
	public async Task<IActionResult> Profile()
	{
		if(User.Identity == null || !User.Identity.IsAuthenticated) 
			return RedirectToAction("Login", "Auth");
			
		return View(await _userServices.GetProfileViewModelByUser(User));
	}

	[HttpGet]
	public async Task<IActionResult> ProfileEdit()
	{
		if(User.Identity == null || !User.Identity.IsAuthenticated) 
			return RedirectToAction("Login", "Auth");
			
		return View(await _userServices.GetProfileViewModelByUser(User));
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
		if(User.Identity == null || !User.Identity.IsAuthenticated) 
			return RedirectToAction("Login", "Auth");

		UserAccount TargetUser = await _userManager.FindByIdAsync(Id);
		UserAccount OwnUser = await _userManager.GetUserAsync(User);

		if(TargetUser == null)
			return RedirectToAction("Index", "Home");

		return View(await _userServices.GetProfileOther(OwnUser, TargetUser));
	}

	[HttpPost]
	public async Task<IActionResult> AddReputation([FromBody] AddReputationDTO Data)
	{
		if(Data == null)
			return BadRequest();

		UserAccount TargetUser = await _userManager.FindByIdAsync(Data.Id.ToString());
		UserAccount OwnUser = await _userManager.GetUserAsync(User);

		if(OwnUser == null || TargetUser == null)
			return NotFound();

		ReputationRelation? RepRlt = await RepDbContext.ReputationRelations
			.Where(rlt => (rlt.UserObj == TargetUser.Id && rlt.UserSub == OwnUser.Id) 
						|| (rlt.UserSub == TargetUser.Id && rlt.UserObj == OwnUser.Id))
			.FirstOrDefaultAsync();

		if(RepRlt == null) // Relation not exist
		{
			await RepDbContext.ReputationRelations.AddAsync(new ReputationRelation()
			{
				Id = Guid.NewGuid(),
				UserSub = OwnUser.Id,
				UserObj = TargetUser.Id,
				Relation = 0b01
			});
		} else if(RepRlt.UserSub == OwnUser.Id)
		{
			RepRlt.Relation ^= 0b01;
			RepDbContext.ReputationRelations.Entry(RepRlt);
		} else if(RepRlt.UserObj == OwnUser.Id)
		{
			RepRlt.Relation ^= 0b10;
			RepDbContext.ReputationRelations.Entry(RepRlt);
		}

		await RepDbContext.SaveChangesAsync();
		return Ok();
	}

	[HttpGet]
	public async Task<int> FindReputation(Guid TargetID)
	{
		return await _userServices.FindUserReputation(TargetID);
	}
}
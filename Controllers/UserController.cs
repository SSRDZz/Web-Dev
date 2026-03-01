using KMITL_WebDev_MiniProject.DTO;
using KMITL_WebDev_MiniProject.Entites;
using KMITL_WebDev_MiniProject.Models;
using KMITL_WebDev_MiniProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KMITL_WebDev_MiniProject.Controllers;
public class UserController(UserManager<UserAccount> userManager, IWebHostEnvironment env, ApplicationReputationsDbContext RepDbContext) : Controller
{
	private UserManager<UserAccount> UserMang {get; init;} = userManager;
	private ApplicationReputationsDbContext RepDbContext {get; init;} = RepDbContext;
	private UserServices UserServ {get; init;} = new UserServices(userManager, env, RepDbContext);
	private FileUploadServcies FUS {get; init;} = new FileUploadServcies(env);

	[HttpGet]
	[Authorize]
	public async Task<IActionResult> Profile()
	{
		return View(await UserServ.GetProfileViewModelByUser(User));
	}

	[HttpGet]
	[Authorize]
	public async Task<IActionResult> ProfileEdit()
	{
		return View(await UserServ.GetProfileViewModelByUser(User));
	}

	[HttpPost]
	[Authorize]
	public async Task<IActionResult> ProfileUpdate(ProfileViewModel model, IFormFile? newPicture)
	{
		if(!ModelState.IsValid)
			return RedirectToAction("Profile");

		UserAccount user = await UserMang.GetUserAsync(User);

		if(user == null)
			return NotFound();

		if(FUS.FileIsExist(newPicture))
		{
			await FUS.Upload(newPicture, user.Id.ToString());
			user.ImagePath = Path.Combine("image", "UserProfile", $"{user.Id}{FUS.LastExt}");
		}

		user.FirstName = model.FirstName;
		user.LastName  = model.LastName;

		IdentityResult res = await UserMang.UpdateAsync(user);

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
	[Authorize]
	public async Task<IActionResult> ProfileOther(string Id)
	{

		UserAccount TargetUser = await UserMang.FindByIdAsync(Id);
		UserAccount OwnUser = await UserMang.GetUserAsync(User);

		if(TargetUser == null)
			return RedirectToAction("Index", "Home");

		return View(await UserServ.GetProfileOther(OwnUser, TargetUser));
	}

	[HttpPost]
	[Authorize]
	public async Task<IActionResult> AddReputation([FromBody] AddReputationDTO Data)
	{
		if(Data == null)
			return BadRequest();

		UserAccount TargetUser = await UserMang.FindByIdAsync(Data.Id.ToString());
		UserAccount OwnUser = await UserMang.GetUserAsync(User);

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
	[Authorize]
	public async Task<int> FindReputation(Guid TargetID)
	{
		return await UserServ.FindUserReputation(TargetID);
	}
}
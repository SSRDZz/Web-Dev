using System.Security.Claims;
using KMITL_WebDev_MiniProject.Entites;
using KMITL_WebDev_MiniProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KMITL_WebDev_MiniProject.Services;
public class UserServices
{
	private readonly UserManager<UserAccount> UserMang;
	public readonly string GuestImagePath;
	private readonly ApplicationUserUtilDbContext RepDbContext;
	private readonly IWebHostEnvironment Env;

	public UserServices(UserManager<UserAccount> um, IWebHostEnvironment env, ApplicationUserUtilDbContext _RepDbContext)
	{
		UserMang = um;
		RepDbContext = _RepDbContext;
		Env = env;
		GuestImagePath = Path.Combine("image", "UserProfile", "guest_picture.jpg");
	}

	public async Task<ProfileViewModel> GetProfileViewModelByUser(ClaimsPrincipal User)
	{
		UserAccount account = await UserMang.GetUserAsync(User);
		await CheckProfileExist(account);
		return await ToProfileViewModel(account);
	}

	public async Task<ProfileViewModel> ToProfileViewModel(UserAccount Acnt)
	{
		return new ProfileViewModel()
		{
			Id = Acnt.Id,
			FirstName = Acnt.FirstName,
			LastName = Acnt.LastName,
			RealUserName = Acnt.RealUserName,
			Reputation = await FindUserReputation(Acnt.Id),
			ImagePath = Acnt.ImagePath,
			Sex = ((SexTranslator)Acnt.Sex).ToString(),
		};
	}

	public async Task<ProfileOtherViewModel> GetProfileOther(UserAccount OwnUser, UserAccount TargetUser)
	{
		await CheckProfileExist(TargetUser);
		return new ProfileOtherViewModel()
		{
			Id = TargetUser.Id,
			FirstName = TargetUser.FirstName,
			LastName = TargetUser.LastName,
			RealUserName =TargetUser.RealUserName,
			Reputation = await FindUserReputation(TargetUser.Id),
			ImagePath = TargetUser.ImagePath,
			Sex = ((SexTranslator)TargetUser.Sex).ToString(),
			IsLike = await FindIsLike(OwnUser.Id, TargetUser.Id)
		};
	}

	public UserAccount RegisterViewModelToAccount(RegisterViewModel Model)
	{
		return  new UserAccount()
		{
			UserName = Model.Email,
			RealUserName = Model.UserName,
			Email = Model.Email,
			FirstName = Model.FirstName,
			LastName = Model.LastName,
			PhoneNumber = Model.PhoneNumber,
			DateOfBirth = Model.DateOfBirth,
			ImagePath = GuestImagePath,
			EmailConfirmed = false
		};
	}

	public async Task<bool> IsRealNameExist(string RealName)
	{
		return await UserMang.Users.AnyAsync(u => u.RealUserName == RealName);
	}

	public async Task<float> FindUserReputation(Guid Id)
	{
		List<ReputationRelation> RepRltList = await RepDbContext.ReputationRelations
			.Where(rlt => rlt.UserSub == Id || rlt.UserObj == Id)
			.ToListAsync();

		if(!RepRltList.Any())
			return 0;

		int sum = 0;

		foreach(ReputationRelation Rlt in RepRltList)
		{
			Console.WriteLine(Rlt.Relation);
			if(Rlt.UserSub == Id)
				sum += (Rlt.Relation & 0xf0) >> 4;
			else	
				sum += Rlt.Relation & 0x0f;
		}
		Console.Write("Sum: ");
		Console.WriteLine(sum);
		return (float)sum/RepRltList.Count;
	}

	public async Task<int> FindIsLike(Guid OwnID, Guid TargetID)
	{
		ReputationRelation? RepRlt = await RepDbContext.ReputationRelations
			.Where(rlt => (rlt.UserObj == OwnID && rlt.UserSub == TargetID) || (rlt.UserSub == OwnID && rlt.UserObj == TargetID))
			.FirstOrDefaultAsync();
		
		if(RepRlt == null)
			return 0;
		else if(RepRlt.UserSub == OwnID && (RepRlt.Relation & 0x0f) > 0)
			return RepRlt.Relation & 0x0f;
		else if(RepRlt.UserObj == OwnID && ((RepRlt.Relation & 0xf0) >> 4) > 0)
			return (RepRlt.Relation & 0xf0) >> 4;

		return 0;
	}

	public async Task CheckProfileExist(UserAccount Acnt)
	{
		if(!File.Exists(Path.Combine(Env.WebRootPath, Acnt.ImagePath)))
		{
			Acnt.ImagePath = GuestImagePath;
			await UserMang.UpdateAsync(Acnt);
		}
	}
}

public enum SexTranslator
{
	Male,
	Female,
	Other,
}
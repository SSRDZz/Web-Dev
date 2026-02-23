using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using KMITL_WebDev_MiniProject.Entites;
using KMITL_WebDev_MiniProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;

namespace KMITL_WebDev_MiniProject.Services;
public class UserServices
{
	private readonly UserManager<UserAccount> _userManager;
	public readonly string guestImageURL;
	private readonly ApplicationReputationsDbContext RepDbContext;

	public UserServices(UserManager<UserAccount> um, IWebHostEnvironment env, ApplicationReputationsDbContext _RepDbContext)
	{
		_userManager = um;
		RepDbContext = _RepDbContext;

		string path = Path.Combine(env.WebRootPath, "image", "guest_picture.jpg");
		byte[] fileBytes = File.ReadAllBytesAsync(path).Result;
		string base64Form = Convert.ToBase64String(fileBytes);

		guestImageURL = base64Form;
	}

	private async Task<UserAccount> GetAccountByUser(ClaimsPrincipal User)
	{
		return await _userManager.GetUserAsync(User);
	}

	public async Task<ProfileViewModel> GetProfileViewModelByUser(ClaimsPrincipal User)
	{
		UserAccount account = await GetAccountByUser(User);
		return await ToProfileViewModel(account);
	}

	public async Task<ProfileViewModel> ToProfileViewModel(UserAccount acc)
	{
		return new ProfileViewModel()
		{
			Id = acc.Id,
			FirstName = acc.FirstName,
			LastName = acc.LastName,
			RealUserName = acc.RealUserName,
			Reputation = await FindUserReputation(acc.Id),
			ImageURL = acc.ImageURL,
			Sex = ((SexTranslator)acc.Sex).ToString(),
		};
	}

	public async Task<ProfileOtherViewModel> GetProfileOther(UserAccount OwnUser, UserAccount TargetUser)
	{
		return new ProfileOtherViewModel()
		{
			Id = TargetUser.Id,
			FirstName = TargetUser.FirstName,
			LastName = TargetUser.LastName,
			RealUserName =TargetUser.RealUserName,
			Reputation = await FindUserReputation(TargetUser.Id),
			ImageURL = TargetUser.ImageURL,
			Sex = ((SexTranslator)TargetUser.Sex).ToString(),
			IsLike = await FindIsLike(OwnUser.Id, TargetUser.Id)
		};
	}

	public async Task<string?> ImageFileToBase64(IFormFile? picture)
	{
		if(picture != null && picture.Length > 0)
		{
			using (var memoryStream = new MemoryStream())
			{
				await picture.CopyToAsync(memoryStream);
				return Convert.ToBase64String(memoryStream.ToArray());
			}
		}
		return null;
	}

	public UserAccount RegisterViewModelToAccount(RegisterViewModel model)
	{
		return  new UserAccount()
		{
			UserName = model.Email,
			RealUserName = model.UserName,
			Email = model.Email,
			FirstName = model.FirstName,
			LastName = model.LastName,
			PhoneNumber = model.PhoneNumber,
			DateOfBirth = model.DateOfBirth,
			ImageURL = !string.IsNullOrEmpty(model.ImageURL) ? model.ImageURL : guestImageURL,
			EmailConfirmed = false
		};

	}

	public async Task<bool> IsRealNameExist(string RealName)
	{
		return await _userManager.Users.AnyAsync(u => u.RealUserName == RealName);
	}

	public async Task<int> FindUserReputation(Guid Id)
	{
		List<ReputationRelation> RepRltList = await RepDbContext.ReputationRelations
			.Where(rlt => rlt.UserSub == Id || rlt.UserObj == Id)
			.ToListAsync();

		if(!RepRltList.Any())
			return 0;

		int sum = 0;

		foreach(ReputationRelation Rlt in RepRltList)
		{
			if(Rlt.UserSub == Id)
				sum += (Rlt.Relation & 0b10) >> 1;
			else
				sum += Rlt.Relation & 0b01;
		}

		return sum;
	}

	public async Task<bool> FindIsLike(Guid OwnID, Guid TargetID)
	{
		ReputationRelation? RepRlt = await RepDbContext.ReputationRelations
			.Where(rlt => (rlt.UserObj == OwnID && rlt.UserSub == TargetID) || (rlt.UserSub == OwnID && rlt.UserObj == TargetID))
			.FirstOrDefaultAsync();
		
		if(RepRlt == null)
			return false;
		else if((RepRlt.UserSub == OwnID && (RepRlt.Relation & 0b01) == 0b01) || 
				(RepRlt.UserObj == OwnID && (RepRlt.Relation & 0b10) == 0b10))
			return true;
		else 
			return false;
	}
}

public enum SexTranslator
{
	Male,
	Female,
	Other,
}
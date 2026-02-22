using System.Security.Claims;
using KMITL_WebDev_MiniProject.Entites;
using KMITL_WebDev_MiniProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KMITL_WebDev_MiniProject.Services;
public class UserServices
{
	private readonly UserManager<UserAccount> _userManager;
	public readonly string guestImageURL;

	public UserServices(UserManager<UserAccount> um, IWebHostEnvironment env)
	{
		_userManager = um;

		string path = Path.Combine(env.WebRootPath, "image", "guest_picture.jpg");
		byte[] fileBytes = File.ReadAllBytesAsync(path).Result;
		string base64Form = Convert.ToBase64String(fileBytes);

		guestImageURL = base64Form;
	}

	private async Task<UserAccount> getAccountByUser(ClaimsPrincipal User)
	{
		return await _userManager.GetUserAsync(User);
	}

	public async Task<ProfileViewModel> getProfileViewModelByUser(ClaimsPrincipal User)
	{
		UserAccount account = await getAccountByUser(User);
		return ToProfileViewModel(account);
	}

	public ProfileViewModel ToProfileViewModel(UserAccount acc)
	{
		return new ProfileViewModel()
		{
			Id = acc.Id,
			FirstName = acc.FirstName,
			LastName = acc.LastName,
			Reputation = acc.Reputation,
			ImageURL = acc.ImageURL,
			Sex = ((SexTranslator)acc.Sex).ToString()
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
}

public enum SexTranslator
{
	Male,
	Female,
	Other,
}
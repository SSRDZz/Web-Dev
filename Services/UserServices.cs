using System;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using KMITL_WebDev_MiniProject.Data;
using KMITL_WebDev_MiniProject.Entites;
using KMITL_WebDev_MiniProject.Models;
using Microsoft.AspNetCore.Identity;

namespace KMITL_WebDev_MiniProject.Services
{
	public class UserServices
	{
		private readonly UserManager<UserAccount> _userManager;
		private readonly ApplicationDbContext _dbContext;
		public readonly string guestImageURL;

		public UserServices(UserManager<UserAccount> um, ApplicationDbContext dbc, IWebHostEnvironment env)
		{
			_userManager = um;
			_dbContext = dbc;

			string path = Path.Combine(env.ContentRootPath, "Contents", "images", "guest_picture.jpg");
			byte[] fileBytes = System.IO.File.ReadAllBytesAsync(path).Result;
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
				FirstName = acc.FirstName,
				LastName = acc.LastName,
				Reputation = acc.Reputation,
				ImageURL = acc.ImageURL,
				Sex = ((SexTranslator)acc.Sex).ToString()
			};
		}

		public async Task<string> ImageFileToBase64(IFormFile picture)
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
	}

	
}

public enum SexTranslator
{
	Male,
	Female,
	Other,
}
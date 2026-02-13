using System.Security.Claims;
using KMITL_WebDev_MiniProject.Data;
using KMITL_WebDev_MiniProject.Entites;
using KMITL_WebDev_MiniProject.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;

namespace KMITL_WebDev_MiniProject.Services
{
	public class UserServices
	{
		private readonly UserManager<UserAccount> _userManager;
		private readonly ApplicationDbContext _dbContext;

		public UserServices(UserManager<UserAccount> um, ApplicationDbContext dbc)
		{
			_userManager = um;
			_dbContext = dbc;
		}

		private async Task<UserAccount> getAccountByUser(ClaimsPrincipal User)
		{
			return await _userManager.GetUserAsync(User);
		}

		public ProfileViewModel getProfileViewModelByUser(ClaimsPrincipal User)
		{
			UserAccount account = getAccountByUser(User).Result;
			return ToProfileViewModel(account);
			
		}

		public ProfileViewModel ToProfileViewModel(UserAccount acc)
		{
			return new ProfileViewModel()
			{
				FirstName = acc.FirstName,
				LastName = acc.LastName,
				Reputation = acc.Reputation
			};
		}
	}
}
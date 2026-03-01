using KMITL_WebDev_MiniProject.Data;
using KMITL_WebDev_MiniProject.Entites;
using KMITL_WebDev_MiniProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KMITL_WebDev_MiniProject.Controllers;
public class SearchController(ApplicationUsersDbContext dbContext, IWebHostEnvironment Env) : Controller
{
	private ApplicationUsersDbContext dbContext {get; init;} = dbContext;
	private IWebHostEnvironment Env {get; init;} = Env;
	public void Search()
	{
		// Search with Username and Postname
	}

	[HttpGet]
	[Authorize]
	public IActionResult SearchByUsername(string Username)
	{
		if(string.IsNullOrEmpty(Username))
			return PartialView(new List<SearchUserViewModel>());

		List<UserAccount> users = dbContext.Users.Where(u => u.RealUserName.Contains(Username)).ToList();
		List<SearchUserViewModel> res = new List<SearchUserViewModel>();

		for(int i = 0; i < users.Count; i++)
		{
			res.Add(new SearchUserViewModel()
			{
				Id = users[i].Id,
				Username = users[i].RealUserName,
				ImagePath = users[i].ImagePath
			});
		}

		return PartialView(res);
	}
};
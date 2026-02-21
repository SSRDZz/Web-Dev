using KMITL_WebDev_MiniProject.Data;
using KMITL_WebDev_MiniProject.Entites;
using Microsoft.AspNetCore.Mvc;

namespace KMITL_WebDev_MiniProject.Controllers;
public class SearchController(ApplicationDbContext dbContext) : Controller
{
	private ApplicationDbContext dbContext {get; init;} = dbContext;
	public void Search()
	{
		
	}

	[HttpPost]
	public IActionResult SearchByUsername(string Username)
	{
		if(string.IsNullOrEmpty(Username))
			return PartialView(new List<UserAccount>());

		List<UserAccount> users = dbContext.Users.Where(u => u.RealUserName.Contains(Username)).ToList();
		return PartialView(users);
	}
};
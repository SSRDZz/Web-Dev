using KMITL_WebDev_MiniProject.Data;
using KMITL_WebDev_MiniProject.Entites;
using KMITL_WebDev_MiniProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace KMITL_WebDev_MiniProject.Controllers;
public class SearchController(ApplicationDbContext dbContext) : Controller
{
	private ApplicationDbContext dbContext {get; init;} = dbContext;

	[Route("Search")]
	[Route("Search/Index")]	
	[HttpGet]
	public IActionResult Index()
	{
		if(User.Identity == null || !User.Identity.IsAuthenticated) // ต้อง authen ก่อน 
		{
			return RedirectToAction("Login","Auth"); // back to default
		}

		return View();
	}
	

	
	public void Search()
	{
		// Search with Username and Postname
	}

	[HttpPost]
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
				Id = users[i].Id.ToString(),
				Username = users[i].RealUserName,
				ImageURL = users[i].ImageURL
			});
		}

		return PartialView(res);
	}
};
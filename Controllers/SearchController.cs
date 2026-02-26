using System.Security.Cryptography.X509Certificates;
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
	
	[HttpGet]			// end point (server side to pass data from DB)
	public IActionResult GetData(string keyword, string type)
	{

		var mockEvents = new List<object> // mock ไว้ก่อน
		{
			new { message = $"search:{keyword} type:{type}"},
			new { title = $"{keyword} Workshop in ESL", date = "2026-03-10", location = "Indonesia"},
			new { title = $"{keyword} ISAG Group Meetup", date = "2026-03-15", location = "Thailand"}
		};

		return Json(mockEvents);

	}

	// not using now
	public class SearchRequest
	{
		// make ? so it can be null or using require
		public string ?Keyword {get; set;} 
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
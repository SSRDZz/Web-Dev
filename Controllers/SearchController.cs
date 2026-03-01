using System.Security.Cryptography.X509Certificates;
using KMITL_WebDev_MiniProject.Data;
using KMITL_WebDev_MiniProject.Entites;
using KMITL_WebDev_MiniProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KMITL_WebDev_MiniProject.Controllers;
public class SearchController(ApplicationUsersDbContext dbContext, IWebHostEnvironment Env) : Controller
{
	private ApplicationUsersDbContext dbContext {get; init;} = dbContext;

	[Authorize]
	[Route("Search")]
	[Route("Search/Index")]	
	[HttpGet]
	public IActionResult Index(string? keyword)
	{
		if(User.Identity == null || !User.Identity.IsAuthenticated) // ต้อง authen ก่อน 
		{
			return RedirectToAction("Login","Auth"); // back to default
		}
		ViewData["CurrentKeyword"] = keyword;

		return View();
	}
	
	[HttpGet]			
	[Produces("application/json")] // end point (server side to pass data from DB)
	[Authorize]
	public IActionResult GetData(string? keyword, string? type)
	{
		keyword = keyword?.Trim() ?? "*"; // means if it null -> make keyword = *
		type = type ?? "All";

		// Console.Write(keyword + type + "\n");

        var response = new SearchResponse { Message = $"search:{keyword} type:{type}" };

        response.Activity = new List<object> // mock list ไว้ก่อน
		{
			new { title = $"{keyword} Workshop in ESL", date = "2026-03-10", location = "Indonesia"},
			new { title = $"{keyword} ISAG Group Meetup", date = "2026-03-15", location = "Thailand"},
			new { title = $"{keyword}-{type} -> From search web", date = "2026-03-15", location = "Thailand"}
		};

		return Json(response);

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
using System.Security.Cryptography.X509Certificates;
using KMITL_WebDev_MiniProject.Data;
using KMITL_WebDev_MiniProject.Entites;
using KMITL_WebDev_MiniProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KMITL_WebDev_MiniProject.Controllers;
public class SearchController(ApplicationUsersDbContext dbContext, ApplicationActivitiesDbContext dbContext2,IWebHostEnvironment Env) : Controller
{
	private ApplicationUsersDbContext dbContext {get; init;} = dbContext;
	private ApplicationActivitiesDbContext dbContext2 {get; init;} = dbContext2;

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
		keyword = keyword?.Trim() ?? ""; // means if it null -> make keyword = *
		type = type ?? "All";

        // var response = new SearchResponse { Message = $"search:{keyword} type:{type}" };
        var response = new SearchResponse {Message = new {keyword = $"{keyword}", type = $"{type}"} };

		List<UserAccount> user_query;
		// List Activity
		List<Activity> activities_query;

		if(type=="People" || type == "All")
		{
			user_query = dbContext.Users
				.Where(u =>
					string.IsNullOrEmpty(keyword) ||
					(u.RealUserName != null && u.RealUserName.Contains(keyword)) ||
					(u.UserName != null && u.UserName.Contains(keyword)))
				.ToList();		// query user
			foreach (var user in user_query)
			{
				response.Result_User.Add(				// add in json
					new { name = user.RealUserName ?? user.UserName ?? "Unknown", image = $"{Url.Content("~/" +  user.ImagePath)}", id = $"{user.Id}"}
				);
				// Console.Write(Url.Content("~/" +  user.ImagePath) +"|||||||||||||||||"+ user.ImagePath + "-----------------------\n");
			}
		}
		if(type=="Activity" || type == "All")
		{
			activities_query = dbContext2.Activities
				.Where(a =>
					string.IsNullOrEmpty(keyword) ||
					EF.Functions.Like(a.Name, $"%{keyword}%") ||
					(a.KeywordsText != null && EF.Functions.Like(a.KeywordsText, $"%{keyword}%"))
					)
				.ToList();
			foreach (var activity in activities_query)
			{
				response.Result_Activity.Add(
					new { name = $"{activity.Name}", date = activity.EventDate.ToString("yyyy-MM-dd"), location = activity.Location, id = activity.Id }
				);
			}
		}


		return Json(response);

	}
};
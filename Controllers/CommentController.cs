using Microsoft.AspNetCore.Mvc;
using KMITL_WebDev_MiniProject.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using KMITL_WebDev_MiniProject.DTO;
using KMITL_WebDev_MiniProject.Services;

namespace KMITL_WebDev_MiniProject.Controllers;
public class CommentController(ApplicationUserUtilDbContext UserUtilDbContext, UserManager<UserAccount> UserMang) : Controller
{
	private CommentServices ComSer {get; init;} = new CommentServices(UserUtilDbContext, UserMang);

	[HttpGet]
	[Authorize]
	public async Task<List<ShowCommentDTO>> FindComment(Guid ActivityID)
	{
		return await ComSer.ShowCommentDTOs(ActivityID);
	}

	[HttpPost]
	[Authorize]
	public async Task<IActionResult> CreateComment([FromBody] AddCommentDTO Data)
	{
		UserAccount Owner = await UserMang.GetUserAsync(User); 
		if(Owner == null || Data == null)
			return BadRequest();
		Comment com = new Comment()
		{
			OwnerID = Owner.Id,
			ActivityID = Data.ActivityID,
			Content = Data.Content
		};
		await UserUtilDbContext.Comments.AddAsync(com);
		await UserUtilDbContext.SaveChangesAsync();
		return PartialView("~/Views/Activity/Comment.cshtml", await ComSer.ShowCommentDTOs(Data.ActivityID));
	}
}
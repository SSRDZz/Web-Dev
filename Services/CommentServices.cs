using KMITL_WebDev_MiniProject.DTO;
using KMITL_WebDev_MiniProject.Entites;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KMITL_WebDev_MiniProject.Services;
public class CommentServices(ApplicationUserUtilDbContext UserUtilDbContext, UserManager<UserAccount> UserMang)
{
	private ApplicationUserUtilDbContext UserUtilDbContext {get; init;} = UserUtilDbContext;
	private UserManager<UserAccount> UserMang {get; init;} = UserMang;

	public async Task<List<Comment>> FindComment(Guid ActivityID)
	{
		List<Comment> Comments = await UserUtilDbContext.Comments.Where(com => com.ActivityID == ActivityID).ToListAsync();
		return Comments;
	}

	public  async Task<List<ShowCommentDTO>> ShowCommentDTOs(Guid ActivityID)
	{
		List<Comment> Comments = await FindComment(ActivityID);
		List<ShowCommentDTO> Res = new List<ShowCommentDTO>();
		foreach(Comment com in Comments)
		{
			UserAccount Owner = await UserMang.FindByIdAsync(com.OwnerID.ToString());
			Res.Add(new ShowCommentDTO(){ UserName = Owner.RealUserName, Content = com.Content, ImagePath = Owner.ImagePath});
		}
		return Res;
	}
}
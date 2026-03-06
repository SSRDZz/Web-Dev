using System.ComponentModel.DataAnnotations;

namespace KMITL_WebDev_MiniProject.DTO;
public class AddCommentDTO
{
	[Required]
	public Guid ActivityID {get; set;}

	[Required]
	public string Content {get; set;}
}
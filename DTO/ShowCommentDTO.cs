using System.ComponentModel.DataAnnotations;

namespace KMITL_WebDev_MiniProject.DTO;
public class ShowCommentDTO
{
	[Required]
	public string UserName {get; set;}

	[Required]
	public string Content {get; set;}

	[Required]
	public string ImagePath {get; set;}
}
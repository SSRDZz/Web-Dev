using System.ComponentModel.DataAnnotations;

namespace KMITL_WebDev_MiniProject.Entites;
public class Comment
{
	[Key]
	[Required]
	public Guid Id {get; set;}

	[Required]
	public Guid OwnerID {get; set;}

	[Required]
	public Guid ActivityID {get; set;}

	[Required]
	public string Content {get; set;}
}
using System.ComponentModel.DataAnnotations;
using KMITL_WebDev_MiniProject.Entites;

namespace KMITL_WebDev_MiniProject.DTO;
public class ActivityDTO
{
	[Required]
	public Activity Act {get; set; }

	[Required]
	public List<ShowCommentDTO> Comments { get; set; }

	public string OwnerName { get; set; } = string.Empty;

	// profile image path of the activity owner
	public string OwnerImagePath { get; set; } = string.Empty;

	public bool IsJoined { get; set; }

	public bool IsOwner { get; set; }

	public bool IsLike {get; set;}

	public int LikeCount {get; set;}
}
using System.ComponentModel.DataAnnotations;
using KMITL_WebDev_MiniProject.Entites;

namespace KMITL_WebDev_MiniProject.DTO;
public class ActivityDTO
{
	[Required]
	public Activity Act {get; set; }

	[Required]
	public List<ShowCommentDTO> Comments { get; set; }

	// display name of the activity owner (pulled from UserAccount)
	public string OwnerName { get; set; } = string.Empty;
}
using System.ComponentModel.DataAnnotations;

namespace KMITL_WebDev_MiniProject.DTO;
public class AddReputationDTO
{
	[Required]
	public Guid Id {get; set;}

	[Required]
	public bool IsLike {get; set;}
}
using System.ComponentModel.DataAnnotations;

namespace KMITL_WebDev_MiniProject.Models;
public class ProfileOtherViewModel: ProfileViewModel
{
	[Required]
	public bool IsLike {get; set;}
}
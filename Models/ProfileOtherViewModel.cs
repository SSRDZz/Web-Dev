using System.ComponentModel.DataAnnotations;

namespace KMITL_WebDev_MiniProject.Models;
public class ProfileOtherViewModel: ProfileViewModel
{
	[Required]
	public int IsLike {get; set;}
}
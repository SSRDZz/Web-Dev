using System.ComponentModel.DataAnnotations;

namespace KMITL_WebDev_MiniProject.Models;
public class SearchUserViewModel
{
	[Required]
	public Guid Id {get; set;}
	
	[Required]
	public string Username {get; set;}

	[Required]
	public string ImageURL {get; set;}
};
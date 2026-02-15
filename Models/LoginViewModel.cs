using System.ComponentModel.DataAnnotations;

namespace KMITL_WebDev_MiniProject.Models
{
	public class LoginViewModel
	{
		[Required]
		[EmailAddress]
		[Display(Name="Email Address")]
		public string Email {get; set;}	

		[Required]
		[DataType(DataType.Password)]
		public string Password {get; set;}

		[Display(Name="Keep me logged in")]
		public bool RememberMe {get; set;}
	}
}
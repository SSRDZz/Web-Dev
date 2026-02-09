using System.ComponentModel.DataAnnotations;

namespace KMITL_WebDev_MiniProject.Models
{
	public class LoginViewModel
	{
		[Required]
		[EmailAddress]
		[Display(Name="Your Email")]
		public string Email {get; set;}	

		[Required]
		[DataType(DataType.Password)]
		public string Password {get; set;}

		[Display(Name="Remember me?")]
		public bool RememberMe {get; set;}
	}
}
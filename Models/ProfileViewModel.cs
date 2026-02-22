using System.ComponentModel.DataAnnotations;

namespace KMITL_WebDev_MiniProject.Models
{
	public class ProfileViewModel
	{
		[Required]
		public Guid Id {get; set;}

		[Required(ErrorMessage="FirstName Error")]
		public string FirstName {get; set;}

		[Required(ErrorMessage="LastName Error")]
		public string LastName {get; set;}

		[Required]
		public string RealUserName {get; set;}

		[Required(ErrorMessage="Reputation Error")]
		public int Reputation {get; set;}
		
		[Required]
		public string ImageURL {get; set;}

		[Required]
		public string Sex {get; set;}
	}
}
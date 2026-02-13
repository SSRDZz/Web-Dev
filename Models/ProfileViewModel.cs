using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;

namespace KMITL_WebDev_MiniProject.Models
{
	public class ProfileViewModel
	{
		[Required]
		public string FirstName {get; set;}

		[Required]
		public string LastName {get; set;}

		[Required]
		public int Reputation {get; set;}
		
		public string ImageURL {get; set;}
	}
}
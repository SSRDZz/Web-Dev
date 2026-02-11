using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KMITL_WebDev_MiniProject.Entites
{
	[Index(nameof(Email), IsUnique = true)]
	[Index(nameof(UserName), IsUnique = true)]
	public class UserAccount: IdentityUser
	{
		[Key]
		public int Id {get; set;}

		[Required]
		public string FirstName {get; set;}

		[Required]
		public string LastName {get; set;}

		[Required]
		[DataType(DataType.EmailAddress)]
		public string Email {get; set;}

		[Required]
		public override string UserName {get; set;}
	}
};
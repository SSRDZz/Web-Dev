using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace KMITL_WebDev_MiniProject.Models
{
	[Index(nameof(Email), IsUnique = true)]
	[Index(nameof(UserName), IsUnique = true)]
	public class RegisterViewModel
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
		public string UserName {get; set;}

		[Required]
		[DataType(DataType.Password)]
		public string Password {get; set;}

		[Compare("Password", ErrorMessage="Please confirm your password")]
		[DataType(DataType.Password)]
		public string ConfirmPassword {get; set;}

		[Required]
		public uint Sex {get; set;}

		[Required]
		[DataType(DataType.PhoneNumber)]
		public string PhoneNumber {get; set;}

		[Required]
		[DataType(DataType.Date)]
		public DateOnly DateOfBirth {get; set;}

		[DataType(DataType.ImageUrl)]
		public string? ImageURL {get; set;}
	}
}
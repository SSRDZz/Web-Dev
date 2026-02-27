using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KMITL_WebDev_MiniProject.Entites;

[Index(nameof(RealUserName), IsUnique = true)]
public class UserAccount: IdentityUser<Guid>
{
	[Required]
	public string FirstName {get; set;}

	[Required]
	public string LastName {get; set;}

	[Required]
	public string RealUserName {get; set;}

	[Required]
	public uint Sex {get; set;}

	[Required]
	[DataType(DataType.Date)]
	public DateOnly DateOfBirth {get; set;}

	[Required]
	public string ImagePath {get; set;}
}
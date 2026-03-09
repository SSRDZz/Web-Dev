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

	// many-to-many: activities this user co-owns
	public ICollection<Activity> CoOwnedActivities { get; set; } = new List<Activity>();

	// many-to-many: activities this user is participating in
	public ICollection<Activity> ParticipatingActivities { get; set; } = new List<Activity>();
}
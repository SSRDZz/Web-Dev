using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace KMITL_WebDev_MiniProject.Entites;

public class Activity
{
	[Key]
	public Guid Id { get; set; }

	[Required]
	[StringLength(100, MinimumLength = 3)]
	public string Name { get; set; } = null!;

	public string? Description { get; set; }

	[StringLength(2000)]
	public string? KeywordsText { get; set; }

	[NotMapped]
	public ICollection<string> Keywords => (KeywordsText ?? string.Empty)
		.Split(',', StringSplitOptions.RemoveEmptyEntries)
		.Select(k => k.Trim())
		.Where(k => !string.IsNullOrWhiteSpace(k))
		.ToList();

	[Url]
	public string? ImageUrl { get; set; }

	[Range(1, 1000)]
	public int MaxPeople { get; set; }

	[Required]
	public int RecruitingMode { get; set; }

	public bool ShowParticipants { get; set; } = false;

	[Required]
	public Guid OwnerId { get; set; }

	public ICollection<ActivityUser> ActivityUsers { get; set; } = new List<ActivityUser>();

	[NotMapped]
	public ICollection<UserAccount> CoOwners => ActivityUsers
		.Where(au => au.Role == ActivityUserRole.CoOwner)
		.Select(au => au.User)
		.Where(u => u != null)
		.ToList();

	[NotMapped]
	public ICollection<UserAccount> Participants => ActivityUsers
		.Where(au => au.Role == ActivityUserRole.Participant || au.Role == ActivityUserRole.Locked)
		.Select(au => au.User)
		.Where(u => u != null)
		.ToList();

	[Required]
	public DateTime EventDate { get; set; }

	[Required]
	[StringLength(255)]
	public string Location { get; set; } = null!;

	[StringLength(500)]
	public string? MapUrl { get; set; }

	[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
	public DateTime CreatedAt { get; set; }

	public DateTime UpdatedAt { get; set; }
}

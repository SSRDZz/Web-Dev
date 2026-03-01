using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KMITL_WebDev_MiniProject.Entites;

public class Activity
{
	[Key]
	public int Id { get; set; }

	[Required]
	[StringLength(100, MinimumLength = 3)]
	public string Name { get; set; } = null!;

	public string? Description { get; set; }

	public ICollection<ActivityKeyword> Keywords { get; set; } = new List<ActivityKeyword>();

	[Url]
	public string? ImageUrl { get; set; }

	[Range(1, 1000)]
	public int MaxPeople { get; set; }

	[Required]
	public int RecruitingMode { get; set; }

	public bool ShowParticipants { get; set; } = false;

	[Required]
	public Guid OwnerId { get; set; }

	public ICollection<UserAccount> CoOwners { get; set; } = new List<UserAccount>();

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

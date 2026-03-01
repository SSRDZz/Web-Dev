using System.ComponentModel.DataAnnotations;
using KMITL_WebDev_MiniProject.Entites;

namespace KMITL_WebDev_MiniProject.Models;

public class ActivityViewModel
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
    public RecruitingMode RecruitingMode { get; set; }

    public bool ShowParticipants { get; set; } = false;

    [Required]
    public Guid OwnerId { get; set; }

    public ICollection<UserAccount> CoOwners { get; set; } = new List<UserAccount>();

    [Required]
    public DateTime EventDate { get; set; }
    public string Location { get; set; } = null!;

    public string mapURL { get; set; } = null!;
}

public enum RecruitingMode
{   FirstComeFirstServe = 1,
    RandomOnEventDay = 2,
    OwnerSelect = 3
}
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using KMITL_WebDev_MiniProject.Entites;

namespace KMITL_WebDev_MiniProject.Models;

public class ActivityViewModel
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public ICollection<ActivityKeyword> Keywords { get; set; } = new List<ActivityKeyword>();

    // temporary field for user input; controller should split on commas
    [Display(Name = "Keywords (comma-separated)")]
    public string? KeywordInput { get; set; }

    [Range(1, 1000)]
    public int MaxPeople { get; set; }

    [Required]
    [EnumDataType(typeof(RecruitingMode), ErrorMessage = "Invalid recruiting mode.")]
    public RecruitingMode RecruitingMode { get; set; }

    public bool ShowParticipants { get; set; } = false;

    [Required]
    public Guid OwnerId { get; set; }

    public ICollection<UserAccount> CoOwners { get; set; } = new List<UserAccount>();

    [Required]
    public DateTime EventDate { get; set; }
    public string Location { get; set; } = null!;

    // should match Activity.MapUrl
    public string? MapUrl { get; set; }

    // optional file upload for activity image
    [Display(Name = "Image File (optional)")]
    public IFormFile? ActivityImage { get; set; }
}

public enum RecruitingMode
{   FirstComeFirstServe = 1,
    RandomOnEventDay = 2,
    OwnerSelect = 3
}
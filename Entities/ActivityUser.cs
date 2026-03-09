using System.ComponentModel.DataAnnotations;

namespace KMITL_WebDev_MiniProject.Entites;

public enum ActivityUserRole
{
    CoOwner = 1,
    Participant = 2
}

public class ActivityUser
{
    [Required]
    public Guid ActivityId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public ActivityUserRole Role { get; set; }

    public Activity Activity { get; set; } = null!;
    public UserAccount User { get; set; } = null!;
}
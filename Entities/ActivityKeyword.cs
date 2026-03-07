using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KMITL_WebDev_MiniProject.Entites;

public class ActivityKeyword
{
    [Key]
    [Required]
    public Guid Id { get; set; }

    // use Guid to match Activity.Id and avoid the extra ActivityId1 column
    [Required]
    public Guid ActivityId { get; set; }

    [Required]
    [StringLength(100)]
    public string Keyword { get; set; } = null!;

    // navigation back to the parent activity
    public Activity Activity { get; set; } = null!;
}

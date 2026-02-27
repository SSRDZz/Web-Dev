using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KMITL_WebDev_MiniProject.Entites;

public class ActivityKeyword
{
	[Key]
	[Required]
	public Guid Id { get; set; }

	[Required]
	public int ActivityId { get; set; }

	[Required]
	[StringLength(100)]
	public string Keyword { get; set; } = null!;
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KMITL_WebDev_MiniProject.Entites;
public class ActivityRelation
{
	[Key]
	[Required]
	public Guid Id {get; set;}

	[Required]
	public Guid UserID {get; set;}

	[Required]
	public Guid ActID {get; set;}

	[Required]
	public int Relation {get; set;}
}
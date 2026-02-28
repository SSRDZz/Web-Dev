using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KMITL_WebDev_MiniProject.Entites;
public class ReputationRelation
{
	[Key]
	[Required]
	public Guid Id {get; set;}

	[Required]
	public Guid UserSub {get; set;}

	[Required]
	public Guid UserObj {get; set;}

	[Required]
	public int Relation {get; set;}
}
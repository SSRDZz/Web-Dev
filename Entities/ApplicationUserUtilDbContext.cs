using Microsoft.EntityFrameworkCore;

namespace KMITL_WebDev_MiniProject.Entites;
public class ApplicationUserUtilDbContext : DbContext
{
	public ApplicationUserUtilDbContext(DbContextOptions<ApplicationUserUtilDbContext> options): base(options) {}

	public DbSet<ReputationRelation> ReputationRelations {get; set;}
	public DbSet<Comment> Comments {get; set;}
}
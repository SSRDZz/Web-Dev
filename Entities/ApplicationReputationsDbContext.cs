using Microsoft.EntityFrameworkCore;

namespace KMITL_WebDev_MiniProject.Entites;
public class ApplicationReputationsDbContext : DbContext
{
	public ApplicationReputationsDbContext(DbContextOptions<ApplicationReputationsDbContext> options): base(options) {}

	public DbSet<ReputationRelation> ReputationRelations {get; set;}
}
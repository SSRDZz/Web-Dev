using Microsoft.EntityFrameworkCore;
using KMITL_WebDev_MiniProject.Entites;

namespace KMITL_WebDev_MiniProject.Data;

public class ApplicationActivitiesDbContext : DbContext
{
	public ApplicationActivitiesDbContext(DbContextOptions<ApplicationActivitiesDbContext> options) : base(options) { }

	public DbSet<Activity> Activities { get; set; }
	public DbSet<ActivityKeyword> ActivityKeywords { get; set; }

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.Entity<Activity>(entity =>
		{
			entity.HasKey(a => a.Id);
			entity.Property(a => a.Name).IsRequired().HasMaxLength(100);
			entity.Property(a => a.Location).IsRequired().HasMaxLength(255);
			entity.Property(a => a.OwnerId).IsRequired();
			entity.Property(a => a.EventDate).IsRequired();
			entity.Property(a => a.RecruitingMode).IsRequired();
			entity.HasIndex(a => a.OwnerId);
			entity.HasIndex(a => a.EventDate);
		});

		builder.Entity<ActivityKeyword>(entity =>
		{
			entity.HasKey(k => k.Id);
			entity.Property(k => k.Keyword).IsRequired().HasMaxLength(100);
			entity.Property(k => k.ActivityId).IsRequired();
		});
	}
}

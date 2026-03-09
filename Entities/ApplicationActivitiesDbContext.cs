using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using KMITL_WebDev_MiniProject.Entites;

namespace KMITL_WebDev_MiniProject.Entites;

public class ApplicationActivitiesDbContext : DbContext
{
	public ApplicationActivitiesDbContext(DbContextOptions<ApplicationActivitiesDbContext> options) : base(options) { }

	public DbSet<Activity> Activities { get; set; }
	public DbSet<ActivityUser> ActivityUsers { get; set; }

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.Entity<UserAccount>(entity =>
		{
			entity.ToTable("AspNetUsers");
		});

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

		entity.HasMany(a => a.ActivityUsers)
			.WithOne(au => au.Activity)
			.HasForeignKey(au => au.ActivityId)
			.OnDelete(DeleteBehavior.Cascade);
		});

		builder.Entity<ActivityUser>(entity =>
		{
			entity.ToTable("ActivityUsers");
			entity.HasKey(au => new { au.ActivityId, au.UserId, au.Role });
			entity.Property(au => au.Role).HasConversion<int>();

			entity.HasOne(au => au.User)
				.WithMany(u => u.ActivityUsers)
				.HasForeignKey(au => au.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			entity.HasIndex(au => new { au.UserId, au.Role });
		});
	}
}
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using KMITL_WebDev_MiniProject.Entites;

namespace KMITL_WebDev_MiniProject.Entites;

public class ApplicationActivitiesDbContext : DbContext
{
	public ApplicationActivitiesDbContext(DbContextOptions<ApplicationActivitiesDbContext> options) : base(options) { }

	public DbSet<Activity> Activities { get; set; }
	public DbSet<ActivityKeyword> ActivityKeywords { get; set; }

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

		// configure many-to-many with UserAccount for co‑owners
		entity
			.HasMany(a => a.CoOwners)
			.WithMany(u => u.CoOwnedActivities)
			.UsingEntity(join => join.ToTable("ActivityCoOwners"));

		// configure many-to-many with UserAccount for participants
		entity
			.HasMany(a => a.Participants)
			.WithMany(u => u.ParticipatingActivities)
			.UsingEntity<Dictionary<string, object>>(
				"ActivityParticipants",
				j => j.HasOne<UserAccount>().WithMany().HasForeignKey("ParticipantsId"),
				j => j.HasOne<Activity>().WithMany().HasForeignKey("ActivityId"));
		});
		builder.Entity<ActivityKeyword>(entity =>
		{
			entity.HasKey(k => k.Id);
			entity.Property(k => k.Keyword).IsRequired().HasMaxLength(100);
			entity.Property(k => k.ActivityId).IsRequired();

			// explicit relationship to avoid mismatched column types
			entity.HasOne(k => k.Activity)
				.WithMany(a => a.Keywords)
				.HasForeignKey(k => k.ActivityId)
				.OnDelete(DeleteBehavior.Cascade);
		});
	}
}
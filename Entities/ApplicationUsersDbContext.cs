using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using KMITL_WebDev_MiniProject.Entites;
using Microsoft.AspNetCore.Identity;

namespace KMITL_WebDev_MiniProject.Data;
public class ApplicationUsersDbContext: IdentityDbContext<UserAccount, IdentityRole<Guid>, Guid>
{
    public ApplicationUsersDbContext(DbContextOptions<ApplicationUsersDbContext> options): base(options) {}

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

        builder.Entity<UserAccount>(entity =>
        {
            // Add Property to column Email
            entity.Property(u => u.Email).IsRequired();
            entity.HasIndex(u => u.Email).IsUnique();

            entity.Property(u => u.UserName).IsRequired();
            entity.HasIndex(u => u.UserName).IsUnique();

            entity.Property(u => u.PhoneNumber).HasMaxLength(20);
        });
	}
}
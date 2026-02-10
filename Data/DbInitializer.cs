using System.Runtime.CompilerServices;
using KMITL_WebDev_MiniProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KMITL_WebDev_MiniProject.Data
{
	public class DbInitializer
	{
		public static async Task Initialize(IServiceProvider serviceProvider, UserManager<ApplicationUser> userManager)
		{
			using (var context = new ApplicationDbContext(
				serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()
			))
			{
				context.Database.EnsureCreated();

				// Look for any users.
				if(context.Users.Any())
				{
					return ;
				}

				var user = new ApplicationUser()
				{
					UserName = "testuser@example.com",
					Email = "test@example.com",
					EmailConfirmed = true
				};
				var admin = new ApplicationUser()
				{
					UserName = "admin@example.com",
					Email = "admin@example.com",
					EmailConfirmed = true					
				};

				await userManager.CreateAsync(user, "1234");
				await userManager.CreateAsync(admin, "Admin@1234");
			}
		}
	}
}
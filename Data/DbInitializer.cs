using KMITL_WebDev_MiniProject.Entites;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KMITL_WebDev_MiniProject.Data
{
	public class DbInitializer
	{
		public static async Task Initialize(IServiceProvider serviceProvider, UserManager<UserAccount> userManager)
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
				Console.WriteLine("DB Mocked!");


				var user = new UserAccount()
				{
					FirstName = "Hello",
					LastName = "World",
					UserName = "Testuser@example.com",
					Email = "Testuser@example.com",
					Reputation = 0,
					PhoneNumber = "0811112222",
					DateOfBirth = new DateOnly(2000, 10, 10),
					ImageURL = "",
					EmailConfirmed = false
				};

				// context.Users.Add(user);

				var admin = new UserAccount()
				{
					FirstName = "Admin",
					LastName = "Adder",
					UserName = "Admin@example.com",
					Email = "Admin@example.com",
					Reputation = 0,
					PhoneNumber = "0811112223",
					DateOfBirth = new DateOnly(2001, 10, 10),
					ImageURL = "",
					EmailConfirmed = false	
				};

				var result = await userManager.CreateAsync(user, "Testuser@1234");
				if (!result.Succeeded)
				{
					foreach (IdentityError err in result.Errors)
						Console.WriteLine(err.Code + " : " + err.Description);
				}

				result = await userManager.CreateAsync(admin, "Admin@12345");
				if (!result.Succeeded)
				{
					foreach (var err in result.Errors)
						Console.WriteLine(err.Code + " : " + err.Description);
				}
			}
		}
	}
}
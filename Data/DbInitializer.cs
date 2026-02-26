using KMITL_WebDev_MiniProject.Entites;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KMITL_WebDev_MiniProject.Data;
public class DbInitializer
{
	public static async Task Initialize(IServiceProvider serviceProvider, UserManager<UserAccount> userManager, IWebHostEnvironment env)
	{
		await ForUsers(new ApplicationUsersDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationUsersDbContext>>()), env, userManager);
		await ForReputations(new ApplicationReputationsDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationReputationsDbContext>>()));
	}

	public static async Task ForUsers(ApplicationUsersDbContext context, IWebHostEnvironment env, UserManager<UserAccount> userManager)
	{
		await context.Database.MigrateAsync();
			
		if(context.Users.Any())
			return;

		string guestURL = await GuestImage(env);

		UserAccount test1 = new UserAccount()
		{
			FirstName = "Testing",
			LastName = "Tests",
			RealUserName = "SansTales",
			UserName = "Testuser@example.com",
			Email = "Testuser@example.com",
			Sex = 1,
			PhoneNumber = "0811112222",
			DateOfBirth = new DateOnly(2000, 10, 10),
			ImageURL = guestURL,
			EmailConfirmed = false
		};

		UserAccount admin = new UserAccount()
		{
			FirstName = "Admin",
			LastName = "Endmin",
			RealUserName = "GM",
			UserName = "Admin@example.com",
			Email = "Admin@example.com",
			Sex = 0,
			PhoneNumber = "0811112223",
			DateOfBirth = new DateOnly(2001, 10, 10),
			ImageURL = guestURL,
			EmailConfirmed = false	
		};

		UserAccount user1 = new UserAccount()
		{
			FirstName = "Teew",
			LastName = "Twin",
			RealUserName = "Elaina",
			UserName = "Elaina@gmail.com",
			Email = "Elaina@gmail.com",
			Sex = 1,
			PhoneNumber = "0910001234",
			DateOfBirth = new DateOnly(2000, 10, 11),
			ImageURL = guestURL,
			EmailConfirmed = false	
		};

		UserAccount[] users = {test1, admin, user1};
		string[] passwords = {"Testuser@1234", "Admin@1234", "Elaina@1234"};

		for(int i = 0; i < users.Length; i++)
		{
			var result = await userManager.CreateAsync(users[i], passwords[i]);
			if (!result.Succeeded)
			{
				foreach (IdentityError err in result.Errors)
					Console.WriteLine(err.Code + " : " + err.Description);
			}
		}
	}

	public static async Task ForReputations(ApplicationReputationsDbContext context)
	{
		await context.Database.MigrateAsync();

		if(context.ReputationRelations.Any()) 
			return ;
	} 

	public static async Task<string> GuestImage(IWebHostEnvironment env)
	{
		string path = Path.Combine(env.WebRootPath, "image", "guest_picture.jpg");
		byte[] fileBytes = await File.ReadAllBytesAsync(path);
		string base64Form = Convert.ToBase64String(fileBytes);
		return base64Form;
	}
}
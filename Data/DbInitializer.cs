using KMITL_WebDev_MiniProject.Entites;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KMITL_WebDev_MiniProject.Data;
public class DbInitializer
{
	public static async Task Initialize(IServiceProvider serviceProvider, UserManager<UserAccount> userManager, IWebHostEnvironment env)
	{
		await ForUsers(new ApplicationUsersDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationUsersDbContext>>()), env, userManager);
		await ForUserUtil(new ApplicationUserUtilDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationUserUtilDbContext>>()));
		await ForActivities(new ApplicationActivitiesDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationActivitiesDbContext>>()));
	}

	public static async Task ForUsers(ApplicationUsersDbContext context, IWebHostEnvironment env, UserManager<UserAccount> userManager)
	{
		await context.Database.MigrateAsync();
			
		if(context.Users.Any())
			return;

		string GuestPath = Path.Combine("image", "UserProfile", "guest_picture.jpg");

		UserAccount test1 = new UserAccount()
		{
			FirstName = "Testing",
			LastName = "Tests",
			RealUserName = "SansTales",
			UserName = "Testuser@example.com",
			Email = "Testuser@example.com",
			Sex = 1,
			ImagePath = GuestPath,
			PhoneNumber = "0811112222",
			DateOfBirth = new DateOnly(2000, 10, 10),
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
			ImagePath = GuestPath,
			PhoneNumber = "0811112223",
			DateOfBirth = new DateOnly(2001, 10, 10),
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
			ImagePath = GuestPath,
			PhoneNumber = "0910001234",
			DateOfBirth = new DateOnly(2000, 10, 11),
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

	public static async Task ForUserUtil(ApplicationUserUtilDbContext context)
	{
		await context.Database.MigrateAsync();

		if(context.ReputationRelations.Any()) 
			return ;
	}

	public static async Task ForActivities(ApplicationActivitiesDbContext context)
	{
		await context.Database.MigrateAsync();

		var users = await context.Set<UserAccount>()
			.Select(u => new { u.Id })
			.ToListAsync();

		var userIds = users.Select(u => u.Id).Distinct().ToList();

		if (!userIds.Any())
			return;

		var now = DateTime.Now;

		if(context.Activities.Any())
			return;

		// Add sample activities
		var activities = new List<Activity>
		{
			new Activity
			{
				Name = "Community Cleanup",
				Description = "Join us for a fun community cleanup event!",
				ImageUrl = null,
				MaxPeople = 50,
				RecruitingMode = 1, // FirstComeFirstServe
				ShowParticipants = true,
				OwnerId = PickRandom(userIds),
				EventDate = now.AddDays(7),
				Location = "Central Park",
				MapUrl = "13.7299,100.7788",
				CreatedAt = now,
				UpdatedAt = now
			},
			new Activity
			{
				Name = "Tech Workshop",
				Description = "Learn new programming skills with industry experts",
				ImageUrl = null,
				MaxPeople = 30,
				RecruitingMode = 3, // OwnerSelect
				ShowParticipants = false,
				OwnerId = PickRandom(userIds),
				EventDate = now.AddDays(14),
				Location = "Tech Hub Downtown",
				MapUrl = "13.7367,100.5231",
				CreatedAt = now,
				UpdatedAt = now
			},
			new Activity
			{
				Name = "Morning Run Club",
				Description = "Group running session for all fitness levels.",
				ImageUrl = null,
				MaxPeople = 40,
				RecruitingMode = 1, // FirstComeFirstServe
				ShowParticipants = true,
				OwnerId = PickRandom(userIds),
				EventDate = now.AddDays(3),
				Location = "Lumpini Park",
				MapUrl = "13.7305,100.5418",
				CreatedAt = now,
				UpdatedAt = now
			},
			new Activity
			{
				Name = "Board Game Night",
				Description = "Casual board game meetup with snacks and prizes.",
				ImageUrl = null,
				MaxPeople = 20,
				RecruitingMode = 2, // RandomOnEventDay
				ShowParticipants = true,
				OwnerId = PickRandom(userIds),
				EventDate = now.AddDays(10),
				Location = "Siam Square",
				MapUrl = "13.7448,100.5340",
				CreatedAt = now,
				UpdatedAt = now
			}
		};

		await context.Activities.AddRangeAsync(activities);
		await context.SaveChangesAsync();

		// Add random participants from current users for seeded activities.
		foreach (var activity in activities)
		{
			var candidateUserIds = userIds.Where(id => id != activity.OwnerId).ToList();
			if (!candidateUserIds.Any())
				continue;

			var maxGuests = Math.Max(0, activity.MaxPeople - 1);
			var randomCount = Random.Shared.Next(0, Math.Min(maxGuests, candidateUserIds.Count) + 1);
			var selected = candidateUserIds
				.OrderBy(_ => Guid.NewGuid())
				.Take(randomCount)
				.ToList();

			foreach (var userId in selected)
			{
				context.ActivityUsers.Add(new ActivityUser
				{
					ActivityId = activity.Id,
					UserId = userId,
					Role = activity.RecruitingMode == 3 && Random.Shared.NextDouble() < 0.35
						? ActivityUserRole.Locked
						: ActivityUserRole.Participant
				});
			}
		}

		await context.SaveChangesAsync();
	}

	private static Guid PickRandom(List<Guid> ids)
	{
		return ids[Random.Shared.Next(ids.Count)];
	}
}
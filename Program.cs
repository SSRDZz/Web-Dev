using KMITL_WebDev_MiniProject.Data;
using KMITL_WebDev_MiniProject.Entites;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionStrings = builder.Configuration.GetConnectionString("dbConnection");
var serverVersion = new MySqlServerVersion(new Version(8, 0, 45));

builder.Services.AddDbContext<ApplicationUsersDbContext>(options => options.UseMySql(connectionStrings, serverVersion));
builder.Services.AddDbContext<ApplicationReputationsDbContext>(options => options.UseMySql(connectionStrings, serverVersion));
builder.Services.AddDbContext<ApplicationActivitiesDbContext>(options => options.UseMySql(connectionStrings, serverVersion));

// Config Property of Identity Table
builder.Services.AddIdentity<UserAccount, IdentityRole<Guid>>(options =>
{
    options.User.RequireUniqueEmail = true;

    options.SignIn.RequireConfirmedEmail = false;

    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
}).AddEntityFrameworkStores<ApplicationUsersDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Auth/Login";
    options.AccessDeniedPath = "/Home/Error";
    options.LogoutPath = "/Auth/Logout";
});

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

var app = builder.Build();

// Seed the database with mockup data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<UserAccount>>();
    var env = app.Services.GetRequiredService<IWebHostEnvironment>();
    DbInitializer.Initialize(services, userManager, env).Wait();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();

app.Run();
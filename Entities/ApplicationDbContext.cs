using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using KMITL_WebDev_MiniProject.Models;
using KMITL_WebDev_MiniProject.Entites;

namespace KMITL_WebDev_MiniProject.Data
{
    public class ApplicationDbContext: IdentityDbContext<UserAccount>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options) {}
    }
}
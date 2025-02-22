using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RedMango_API.Models;

namespace RedMango_API.Data
{
    public class AppDBContext:IdentityDbContext<ApplicationUser>
    {
        public AppDBContext(DbContextOptions options):base(options)
        {
            
        }
        public DbSet<ApplicationUser>ApplicationUsers { get; set; }
    }
}

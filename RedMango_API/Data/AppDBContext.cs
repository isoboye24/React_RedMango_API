using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RedMango_API.Data
{
    public class AppDBContext:IdentityDbContext
    {
        public AppDBContext(DbContextOptions options):base(options)
        {
            
        }
    }
}

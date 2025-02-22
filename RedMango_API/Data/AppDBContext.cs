using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RedMango_API.Models;
using static System.Net.Mime.MediaTypeNames;

namespace RedMango_API.Data
{
    public class AppDBContext:IdentityDbContext<ApplicationUser>
    {
        public AppDBContext(DbContextOptions options):base(options)
        {
            
        }
        public DbSet<ApplicationUser>ApplicationUsers { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<MenuItem>().HasData(new MenuItem
            {                    
                MenuItemId = 1,
                Name = "Spring Roll",
                Description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                Image = "../images/spring roll.jpg",
                Price = 7.99,
                Category = "Appetizer",
                SpecialTag = ""
            }, new MenuItem
            {
                MenuItemId = 2,
                Name = "Idli",
                Description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                Image = "../images/idli.jpg",
                Price = 8.99,
                Category = "Appetizer",
                SpecialTag = ""
            }, new MenuItem
            {
                MenuItemId = 3,
                Name = "Panu Puri",
                Description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                Image = "../images/pani puri.jpg",
                Price = 8.99,
                Category = "Appetizer",
                SpecialTag = "Best Seller"
            }, new MenuItem
            {
                MenuItemId = 4,
                Name = "Hakka Noodles",
                Description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                Image = "../images/hakka noodles.jpg",
                Price = 10.99,
                Category = "Entrée",
                SpecialTag = ""
            }, new MenuItem
            {
                MenuItemId = 5,
                Name = "Malai Kofta",
                Description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                Image = "../images/malai kofta.jpg",
                Price = 12.99,
                Category = "Entrée",
                SpecialTag = "Top Rated"
            }, new MenuItem
            {
                MenuItemId = 6,
                Name = "Paneer Pizza",
                Description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                Image = "../images/paneer pizza.jpg",
                Price = 11.99,
                Category = "Entrée",
                SpecialTag = ""
            }, new MenuItem
            {
                MenuItemId = 7,
                Name = "Paneer Tikka",
                Description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                Image = "../images/paneer tikka.jpg",
                Price = 13.99,
                Category = "Entrée",
                SpecialTag = "Chef's Special"
            }, new MenuItem
            {
                MenuItemId = 8,
                Name = "Carrot Love",
                Description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                Image = "../images/carrot love.jpg",
                Price = 4.99,
                Category = "Dessert",
                SpecialTag = ""
            }, new MenuItem
            {
                MenuItemId = 9,
                Name = "Rasmalai",
                Description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                Image = "../images/rasmalai.jpg",
                Price = 4.99,
                Category = "Dessert",
                SpecialTag = "Chef's Special"
            }, new MenuItem
            {
                MenuItemId = 10,
                Name = "Sweet Rolls",
                Description = "Fusc tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                Image = "../images/sweet rolls.jpg",
                Price = 3.99,
                Category = "Dessert",
                SpecialTag = "Top Rated"
            
            });
        }
    }
}

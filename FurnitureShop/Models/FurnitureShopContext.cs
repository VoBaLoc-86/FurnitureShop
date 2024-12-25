using Microsoft.EntityFrameworkCore;
using FurnitureShop.Models;

namespace FurnitureShop.Models
{
    public class FurnitureShopContext : DbContext
    {
        public FurnitureShopContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<User>? Users { get; set; }
        public DbSet<Category>? Categories { get; set; }
        public DbSet<Product>? Products { get; set; }
        public DbSet<Order>? Orders { get; set; }
        public DbSet<Order_Detail>? OrderDetails { get; set; }
        public DbSet<Page>? Pages { get; set; }
        public DbSet<Review>? Reviews { get; set; }
        public DbSet<AdminUser>? AdminUsers { get; set; }
        public DbSet<Feature>? Features { get; set; }
        public DbSet<Setting>? Settings { get; set; }
    }
}

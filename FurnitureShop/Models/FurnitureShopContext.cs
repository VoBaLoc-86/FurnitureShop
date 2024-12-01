using Microsoft.EntityFrameworkCore;

namespace FurnitureShop.Models
{
    public class FurnitureShopContext : DbContext
    {
        public FurnitureShopContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<User>? Users { get; set; }
        public DbSet<Cart>? Carts { get; set; }
        public DbSet<Category>? Categories { get; set; }
        public DbSet<Product>? Products { get; set; }
        public DbSet<Order>? Orders { get; set; }
        public DbSet<Order_Detail>? OrderDetails { get; set; }
        public DbSet<Page>? Pages { get; set; }
        public DbSet<Review>? Reviews { get; set; }
    }
}

namespace FurnitureShop.Models
{
    public class User:BaseModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Email { get; set; }
        public required string Password { get; set; }
        public virtual ICollection<Cart>? Carts { get; set; }
        public virtual ICollection<Order>? Orders { get; set; }
        public virtual ICollection<Review>? Reviews { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureShop.Models
{
    public class Product:BaseModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; } = null;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? Image {  get; set; }
        public int Category_id { get; set; }

        [ForeignKey("Category_id")]
        public virtual Category? Category { get; set; }

        public virtual ICollection<Order_Detail>? Order_Details { get; set; }
        public virtual ICollection<Review>? Reviews { get; set; }
        public virtual ICollection<Cart>? Carts { get; set; }
    }
}

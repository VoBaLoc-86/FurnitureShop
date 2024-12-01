namespace FurnitureShop.Models
{
    public class Category:BaseModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public virtual ICollection<Product>? Products { get; set; }
    }
}

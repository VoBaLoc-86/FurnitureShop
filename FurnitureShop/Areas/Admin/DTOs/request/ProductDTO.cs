using FurnitureShop.Models;

namespace FurnitureShop.Areas.Admin.DTOs.request
{
    public class ProductDTO:BaseModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; } = null;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public IFormFile? Image { get; set; }
        public int Category_id { get; set; }
    }
}

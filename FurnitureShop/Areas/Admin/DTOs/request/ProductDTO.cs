using FurnitureShop.Models;
using System.ComponentModel.DataAnnotations;

namespace FurnitureShop.Areas.Admin.DTOs.request
{
    public class ProductDTO:BaseModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; } = null;

        [Required(ErrorMessage = "Giá sản phẩm là bắt buộc.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn 0.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Số lượng là bắt buộc.")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
        public int Stock { get; set; }
        
        public IFormFile? Image { get; set; }
        public int Category_id { get; set; }
    }
}

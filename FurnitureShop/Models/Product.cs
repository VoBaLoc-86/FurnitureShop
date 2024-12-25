using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureShop.Models
{
    public class Product:BaseModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(40, MinimumLength = 4, ErrorMessage = "Name must be between 4 and 40 characters.")]
        public required string Name { get; set; }
        public string? Description { get; set; } = null;
        [Required(ErrorMessage = "Giá sản phẩm là bắt buộc.")]
        [Range(0.01, 1000000, ErrorMessage = "Giá sản phẩm phải lớn hơn 0 và nhỏ hơn 1 triệu.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Số lượng là bắt buộc.")]
        [Range(1, 10000, ErrorMessage = "Số lượng phải lớn hơn 0 và nhỏ hơn 10.000.")]
        public int Stock { get; set; }
        
        public string? Image {  get; set; }
        [DisplayName("Category Name")]
        public int Category_id { get; set; }

        [ForeignKey("Category_id")]
        public virtual Category? Category { get; set; }

        public virtual ICollection<Order_Detail>? Order_Details { get; set; }
        public virtual ICollection<Review>? Reviews { get; set; }
     
    }
}

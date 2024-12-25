using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FurnitureShop.Models
{
    public class Category:BaseModel
    {
        public int Id { get; set; }
        [DisplayName("Category Name")]
        [Required(ErrorMessage = "Category name is required.")]
        [MaxLength(255, ErrorMessage = "Category name cannot be longer than 255 characters.")]
        [RegularExpression(@"^[a-zA-ZÀ-ỹà-ỹ\s]+$", ErrorMessage = "Category chỉ ghi kí tự")]
        public required string Name { get; set; }


        public virtual ICollection<Product>? Products { get; set; }
    }
}

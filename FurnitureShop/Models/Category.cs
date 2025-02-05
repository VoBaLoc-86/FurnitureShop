using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FurnitureShop.Models
{
    public class Category:BaseModel
    {
        public int Id { get; set; }
        [DisplayName("Category Name")]
        [Required(ErrorMessage = "Category name is required.")]
        [MaxLength(50, ErrorMessage = "Category name cannot be longer than 50 characters.")]
        [MinLength(3, ErrorMessage = "Category name must be at least 3 characters.")]
        [RegularExpression(@"^[a-zA-ZÀ-ỹà-ỹ\s0-9]*$", ErrorMessage = "Name must contain only letters, numbers, and spaces.")]
        [DataType(DataType.Text, ErrorMessage = "Invalid format for category name.")]
        public required string Name { get; set; }


        public virtual ICollection<Product>? Products { get; set; }
    }
}

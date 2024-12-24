using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FurnitureShop.Models
{
    public class Category:BaseModel
    {
        public int Id { get; set; }
        [DisplayName("Category Name")]
        [Required(ErrorMessage = "Category name is required.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Category chỉ ghi kí tự")]
        public required string Name { get; set; }

        public virtual ICollection<Product>? Products { get; set; }
    }
}

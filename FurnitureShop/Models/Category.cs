using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FurnitureShop.Models
{
    public class Category:BaseModel
    {
        public int Id { get; set; }
        [DisplayName("Category Name")]
        public required string Name { get; set; }

        public virtual ICollection<Product>? Products { get; set; }
    }
}

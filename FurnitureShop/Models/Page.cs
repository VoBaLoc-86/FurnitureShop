using System.ComponentModel.DataAnnotations;

namespace FurnitureShop.Models
{
    public class Page:BaseModel
    {
        [Key]
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
    }
}

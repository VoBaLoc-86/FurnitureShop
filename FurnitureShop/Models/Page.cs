using System.ComponentModel.DataAnnotations;

namespace FurnitureShop.Models
{
    public class Page:BaseModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public required string Title { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        [StringLength(500, ErrorMessage = "Content cannot exceed 500 characters.")]
        public required string Content { get; set; }

        
        public string? Image { get; set; }
        [Required(ErrorMessage = "DisplayOrder is required.")]
        public int DisplayOrder { get; set; }
    }
}

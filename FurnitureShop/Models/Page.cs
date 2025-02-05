using System.ComponentModel.DataAnnotations;

namespace FurnitureShop.Models
{
    public class Page:BaseModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required.")]
        [MinLength(3, ErrorMessage = "Title must be at least 3 characters.")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        
        
        public required string Title { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        [StringLength(500, ErrorMessage = "Content cannot exceed 500 characters.")]
        [MinLength(3, ErrorMessage = "Content must be at least 3 characters.")]
        
        [DataType(DataType.Text, ErrorMessage = "Invalid format for Content.")]
        public required string Content { get; set; }

        
        public string? Image { get; set; }
        [Required(ErrorMessage = "DisplayOrder is required.")]
        [Range(0, 100, ErrorMessage = "DisplayOrder phải là số dương và nhỏ hơn 100.")]
        public int DisplayOrder { get; set; }
    }
}

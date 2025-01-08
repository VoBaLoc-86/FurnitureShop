using Humanizer;
using System.ComponentModel.DataAnnotations;

namespace FurnitureShop.Models
{
    public class Feature:BaseModel
    {
        [Key]
        public int FEA_ID { get; set; }
        
        public string? Icon { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(50, ErrorMessage = "Title không được quá 50 kí tự.")]
        public required string Title { get; set; }

        [Required(ErrorMessage = "Subtitle is required.")]
        [StringLength(100, ErrorMessage = "Subtitle không được quá 100 kí tự.")]
        public required string Subtitle { get; set; }
        [Range(0, 100, ErrorMessage = "DisplayOrder phải là số dương và nhỏ hơn 100.")]
        public int DisplayOrder { get; set; }
    }
}

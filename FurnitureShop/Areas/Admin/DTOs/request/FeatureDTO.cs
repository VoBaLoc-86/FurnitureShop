using System.ComponentModel.DataAnnotations;

namespace FurnitureShop.Areas.Admin.DTOs.request
{
    public class FeatureDTO
    {
        
        public int FEA_ID { get; set; }
        
        public required IFormFile Icon { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title must not exceed 100 characters.")]
        public required string Title { get; set; }

        [Required(ErrorMessage = "Subtitle is required.")]
        [StringLength(200, ErrorMessage = "Subtitle must not exceed 200 characters.")]
        public required string Subtitle { get; set; }
        public int DisplayOrder { get; set; }
    }
}

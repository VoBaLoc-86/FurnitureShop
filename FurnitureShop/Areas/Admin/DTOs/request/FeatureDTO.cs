using System.ComponentModel.DataAnnotations;

namespace FurnitureShop.Areas.Admin.DTOs.request
{
    public class FeatureDTO
    {
        
        public int FEA_ID { get; set; }
        public IFormFile? Icon { get; set; }
        public required string Title { get; set; }
        public required string Subtitle { get; set; }
        public int DisplayOrder { get; set; }
    }
}

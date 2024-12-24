using FurnitureShop.Models;
using System.ComponentModel.DataAnnotations;

namespace FurnitureShop.Areas.Admin.DTOs.request
{
    public class BannerDTO:BaseModel
    {
        public int BAN_ID { get; set; }
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Title Banner chỉ ghi kí tự")]
        public required string Title { get; set; }
        public IFormFile? Image { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "DisplayOrder phải lớn hơn 0.")]
        public int DisplayOrder { get; set; }
    }
}

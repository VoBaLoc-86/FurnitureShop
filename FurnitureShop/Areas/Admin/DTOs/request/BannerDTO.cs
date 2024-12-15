using FurnitureShop.Models;

namespace FurnitureShop.Areas.Admin.DTOs.request
{
    public class BannerDTO:BaseModel
    {
        public int BAN_ID { get; set; }
        public required string Title { get; set; }
        public IFormFile? Image { get; set; }
        public int DisplayOrder { get; set; }
    }
}

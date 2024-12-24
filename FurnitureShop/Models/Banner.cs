using Humanizer;
using System.ComponentModel.DataAnnotations;

namespace FurnitureShop.Models
{
    public class Banner:BaseModel
    {
        [Key]
        public int BAN_ID { get; set; }
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Title Banner chỉ ghi kí tự")]
        public required string Title { get; set; }
        public string? Image { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "DisplayOrder phải lớn hơn 0.")]
        public int DisplayOrder { get; set; }
        
    }
}

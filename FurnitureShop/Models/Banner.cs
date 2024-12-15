using Humanizer;
using System.ComponentModel.DataAnnotations;

namespace FurnitureShop.Models
{
    public class Banner:BaseModel
    {
        [Key]
        public int BAN_ID { get; set; }
        public required string Title { get; set; }
        public string Image { get; set; }
        public int DisplayOrder { get; set; }
        
    }
}

using System.ComponentModel;

namespace FurnitureShop.Models
{
    public class BaseModel
    {
        [DisplayName("Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [DisplayName("Created By")]
        public string? CreatedBy { get; set; }
        [DisplayName("Updated Date")]
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        [DisplayName("Updated By")]
        public string? UpdatedBy { get; set; }
    }
}

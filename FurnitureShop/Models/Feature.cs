using Humanizer;
using System.ComponentModel.DataAnnotations;

namespace FurnitureShop.Models
{
    public class Feature:BaseModel
    {
        [Key]
        public int FEA_ID { get; set; }
        public string? Icon { get; set; }
        public required string Title { get; set; }
        public required string Subtitle { get; set; }
        public int DisplayOrder { get; set; }
    }
}

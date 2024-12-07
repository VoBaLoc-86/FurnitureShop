using System.ComponentModel.DataAnnotations;

namespace FurnitureShop.Models
{
    public class Setting:BaseModel
    {
        [Key]
        public int SET_ID { get; set; }
        public required string Name { get; set; }   
        public required string Value { get; set; }
    }
}

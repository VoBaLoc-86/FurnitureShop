using System.ComponentModel.DataAnnotations;

namespace FurnitureShop.Models
{
    public class Setting:BaseModel
    {
        [Key]
        public int SET_ID { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, ErrorMessage = "Name không được vượt quá 50 ký tự.")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Value is required.")]
        [StringLength(200, ErrorMessage = "Value không được vượt quá 200 ký tự.")]
        public required string Value { get; set; }
    }
}

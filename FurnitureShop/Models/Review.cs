using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureShop.Models
{
    public class Review:BaseModel
    {
        public int Id { get; set; }
        public int Product_id { get; set; }
        public int User_id { get; set; }
        public int Rating { get; set; }
        [Required(ErrorMessage = "Comment is required.")]
        [MinLength(3, ErrorMessage = "Comment phải có ít nhất 3 kí tự.")]
        [MaxLength(200, ErrorMessage = "Comment không được vượt quá 200 kí tự.")]
        public required string Comment { get; set; }


        [ForeignKey("Product_id")]
        public virtual Product? Product {  get; set; }
        [ForeignKey("User_id")]
        public virtual User? User { get; set; }
    }
}

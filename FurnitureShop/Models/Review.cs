using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureShop.Models
{
    public class Review:BaseModel
    {
        public int Id { get; set; }
        public int Product_id { get; set; }
        public int User_id { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }

        [ForeignKey("Product_id")]
        public virtual Product? Product {  get; set; }
        [ForeignKey("User_id")]
        public virtual User? User { get; set; }
    }
}

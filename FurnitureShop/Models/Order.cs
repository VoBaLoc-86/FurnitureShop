using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureShop.Models
{
    public class Order:BaseModel
    {
        public int Id { get; set; }
        public int User_id { get; set; }
        public decimal Total_price { get; set; }
        public int Status { get; set; }
        [ForeignKey("User_id")]
        public virtual User? User { get; set; }
        public virtual ICollection<Order_Detail>? Order_Details { get; set; }
    }
}

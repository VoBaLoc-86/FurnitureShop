using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureShop.Models
{
    [Table("Order_Details")]
    public class Order_Detail
    {
        public int Id { get; set; }
        public int Order_id { get; set; }
        public int Product_id { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        [ForeignKey("Order_id")]
        public virtual Order? Order { get; set; }
        [ForeignKey("Product_id")]
        public virtual Product? Product {  get; set; }
    }
}

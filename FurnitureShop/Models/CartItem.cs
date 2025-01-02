namespace FurnitureShop.Models
{
    public class CartItem
    {
        public int Id { get; set; } // ID của sản phẩm
        public  required string Name { get; set; } // Tên sản phẩm
        public required string Image { get; set; } // Hình ảnh sản phẩm
        public decimal Price { get; set; } // Giá sản phẩm
        public int Quantity { get; set; } // Số lượng sản phẩm trong giỏ hàng
    }
}

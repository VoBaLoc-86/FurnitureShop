using FurnitureShop.Models;
using FurnitureShop.Utils;
using Microsoft.AspNetCore.Mvc;

public class CartController : Controller
{
    private readonly FurnitureShopContext _context;

    public CartController(FurnitureShopContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        // Lấy giỏ hàng từ session
        var cart = HttpContext.Session.Get<List<CartItem>>("cart") ?? new List<CartItem>();

        // Lấy danh sách sản phẩm từ cơ sở dữ liệu (hoặc nguồn dữ liệu của bạn)
        var products = _context.Products.ToList(); // Giả sử bạn đang sử dụng Entity Framework để lấy dữ liệu sản phẩm

        // Truyền giỏ hàng và danh sách sản phẩm vào View bằng ViewData
        ViewData["CartItems"] = cart;
        ViewData["Products"] = products; // Thêm thông tin sản phẩm vào ViewData

        return View();
    }

    [HttpPost]
    public IActionResult UpdateCart([FromBody] CartViewModel model)
    {
        // Cập nhật lại giỏ hàng trong session
        HttpContext.Session.Set("cart", model.CartItems);

        return Ok();
    }
    public IActionResult Checkout()
    {
        var userInfo = HttpContext.Session.Get<User>("userInfo");

        if (userInfo == null)
        {
            // Nếu người dùng chưa đăng nhập, chuyển hướng về trang chủ
            return RedirectToAction("Index", "Home");
        }

        // Lấy giỏ hàng từ Session
        var cart = HttpContext.Session.Get<List<CartItem>>("cart");
        if (cart == null || cart.Count == 0)
        {
            // Nếu giỏ hàng rỗng, chuyển hướng về trang giỏ hàng
            TempData["ErrorMessage"] = "Your cart is empty. Please add items to your cart before proceeding.";
            return RedirectToAction("Index", "Cart");
        }

        // Lưu giỏ hàng vào ViewData để sử dụng trong view
        ViewData["CartItems"] = cart;

        // Tính tổng tiền
        var totalAmount = cart.Sum(item => item.Price * item.Quantity);

        ViewData["TotalAmount"] = totalAmount;

        return View();
    }


    public IActionResult Thankyou()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateOrder(string shipping_address, string phone_number)
    {
        if (ModelState.IsValid)
        {
            // Lấy giỏ hàng từ session
            var cart = HttpContext.Session.Get<List<CartItem>>("cart") ?? new List<CartItem>();

            if (cart.Count == 0)
            {
                TempData["ErrorMessage"] = "Your cart is empty!";
                return RedirectToAction("Index", "Home");
            }

            // Lấy thông tin người dùng từ session
            var userInfo = HttpContext.Session.Get<User>("userInfo");

            // Tạo đơn hàng mới
            var order = new Order
            {
                User_id = userInfo.Id,
                Total_price = cart.Sum(item => item.Price * item.Quantity),
                Status = "Pending",
                Shipping_address = shipping_address,
                Payment_status = "Unpaid",
                Phone = phone_number,
                CreatedDate = DateTime.Now,
                CreatedBy = userInfo?.Name,
                UpdatedDate = DateTime.Now,
                UpdatedBy = userInfo?.Name
            };

            // Thêm đơn hàng vào cơ sở dữ liệu
            _context.Add(order);
            await _context.SaveChangesAsync();

            // Thêm chi tiết đơn hàng vào bảng Order_Details
            foreach (var item in cart)
            {
                var orderDetail = new Order_Detail // Sửa lại tên lớp ở đây
                {
                    Order_id = order.Id,
                    Product_id = item.Id,
                    Quantity = item.Quantity,
                    Price = item.Price
                };
                _context.Add(orderDetail);
            }
            await _context.SaveChangesAsync();

            // Xóa giỏ hàng khỏi session sau khi tạo đơn hàng
            HttpContext.Session.Remove("cart");

            TempData["SuccessMessage"] = "Order created successfully!";
            return RedirectToAction("Thankyou","Cart");
        }

        TempData["ErrorMessage"] = "There was an error in your order!";
        return RedirectToAction("Checkout");
    }

}

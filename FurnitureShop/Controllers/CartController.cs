using FurnitureShop.Areas.VNPayAPI.Util;
using FurnitureShop.Models;
using FurnitureShop.Services;
using FurnitureShop.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text;
using System.Web;

public class CartController : Controller
{
    private readonly FurnitureShopContext _context;
    private readonly VNPaySettings _vnpaySettings;

    private readonly IEmailSender _emailSender; 
    
    public CartController(FurnitureShopContext context, IOptions<VNPaySettings> vnpaySettings, IEmailSender emailSender)
    {
        _context = context;
        _vnpaySettings = vnpaySettings.Value;
        _emailSender = emailSender;
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

        // Thiết lập OrderId trong ViewData để sử dụng cho thanh toán
        if (ViewData["OrderId"] == null)
        {
            ViewData["OrderId"] = 0; // Giá trị mặc định
        }

        return View();
    }

    public IActionResult Thankyou()
    {
        return View();
    }
    private string GenerateOrderDetailsHtml(List<CartItem> cart, decimal totalPrice)
    {
        var html = new StringBuilder();
        html.Append("<h1>Xác nhận đơn hàng</h1>");
        html.Append("<p>Cảm ơn bạn đã đặt hàng! Dưới đây là thông tin chi tiết:</p>");
        html.Append("<table style='width:100%; border: 1px solid black; border-collapse: collapse;'>");
        html.Append("<tr><th style='border: 1px solid black; padding: 8px;'>Tên sản phẩm</th><th style='border: 1px solid black; padding: 8px;'>Số lượng</th><th style='border: 1px solid black; padding: 8px;'>Giá</th></tr>");

        foreach (var item in cart)
        {
            html.Append("<tr>");
            html.AppendFormat("<td style='border: 1px solid black; padding: 8px;'>{0}</td>", item.Name);
            html.AppendFormat("<td style='border: 1px solid black; padding: 8px;'>{0}</td>", item.Quantity);
            html.AppendFormat("<td style='border: 1px solid black; padding: 8px;'>{0:C}</td>", item.Price);
            html.Append("</tr>");
        }

        html.AppendFormat("<tr><td colspan='2' style='border: 1px solid black; padding: 8px;'><strong>Tổng tiền</strong></td><td style='border: 1px solid black; padding: 8px;'><strong>{0:C}</strong></td></tr>", totalPrice);
        html.Append("</table>");

        return html.ToString();
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
                TempData["ErrorMessage"] = "Giỏ hàng của bạn trống!";
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
                var orderDetail = new Order_Detail
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

            // Thiết lập OrderId trong ViewData để sử dụng cho thanh toán
            ViewData["OrderId"] = order.Id;

            // Gửi email xác nhận đơn hàng dưới dạng HTML
            var orderDetailsHtml = GenerateOrderDetailsHtml(cart, order.Total_price);
            await _emailSender.SendEmailAsync(
                userInfo.Email,
                "Xác nhận đơn hàng",
                orderDetailsHtml
            );

            // Chuyển hướng đến trang thanh toán để hiển thị nút thanh toán
            return RedirectToAction("Thankyou","Cart");
        }

        return View();
    }


}

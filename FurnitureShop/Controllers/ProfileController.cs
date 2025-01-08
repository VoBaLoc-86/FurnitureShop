using FurnitureShop.Models;
using FurnitureShop.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurnitureShop.Controllers
{
    public class ProfileController : Controller
    {
        private readonly FurnitureShopContext _context;

        public ProfileController(FurnitureShopContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userInfo = HttpContext.Session.Get<User>("userInfo");

            if (userInfo == null)
            {
                return RedirectToAction("Login", "Home"); // Chuyển hướng đến trang đăng nhập nếu chưa đăng nhập
            }
            var userId = userInfo.Id;
            // Truy vấn thông tin người dùng và đơn hàng
            var user = await _context.Users
                .Include(u => u.Orders)
                    .ThenInclude(o => o.Order_Details)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound("Không tìm thấy người dùng.");
            }

            // Trả dữ liệu người dùng đến view
            return View(user);
        }

        [HttpPost("updateUserInfo")]
        public async Task<IActionResult> UpdateUserInfo(
    string? newName, string? newAddress, string? newPhone,
    string? currentPassword, string? newPassword, string? confirmPassword)
        {
            var userInfo = HttpContext.Session.Get<User>("userInfo");

            if (userInfo == null)
            {
                return RedirectToAction("Login", "Home"); // Redirect nếu người dùng chưa đăng nhập
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userInfo.Id);

            if (user == null)
            {
                return NotFound("Không tìm thấy người dùng.");
            }

            // Kiểm tra mật khẩu hiện tại trước khi thay đổi bất kỳ thông tin nào
            if (!string.IsNullOrEmpty(currentPassword) && !BCrypt.Net.BCrypt.Verify(currentPassword, user.Password))
            {
                TempData["Message"] = "Mật khẩu hiện tại không chính xác.";
                return RedirectToAction("Index", "Profile");
            }

            // Xử lý thay đổi mật khẩu
            if (!string.IsNullOrEmpty(newPassword))
            {
                if (newPassword != confirmPassword)
                {
                    TempData["Message"] = "Mật khẩu mới và xác nhận mật khẩu không khớp.";
                    return RedirectToAction("Index", "Profile");
                }

                if (newPassword.Length < 6)
                {
                    TempData["Message"] = "Mật khẩu mới phải có ít nhất 6 ký tự.";
                    return RedirectToAction("Index", "Profile");
                }

                user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                TempData["Message"] = "Cập nhật mật khẩu thành công.";
            }

            // Xử lý thay đổi thông tin người dùng khác
            if (!string.IsNullOrEmpty(newName))
            {
                user.Name = newName;
            }

            if (!string.IsNullOrEmpty(newAddress))
            {
                user.Address = newAddress;
            }

            if (!string.IsNullOrEmpty(newPhone))
            {
                user.Phone = newPhone;
            }

            _context.Update(user);
            await _context.SaveChangesAsync();

            // Cập nhật lại thông tin người dùng trong Session
            HttpContext.Session.Set("userInfo", user);

            TempData["Message"] = "Cập nhật thông tin người dùng thành công.";
            return RedirectToAction("Index", "Profile");
        }



    }
}

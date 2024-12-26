using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FurnitureShop.Models;
using System.Reflection;
using FurnitureShop.Utils;

namespace FurnitureShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminUserController : Controller
    {
        private readonly FurnitureShopContext _context;

        public AdminUserController(FurnitureShopContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminUser
        public async Task<IActionResult> Index()
        {
            return View(await _context.AdminUsers.ToListAsync());
        }

        public IActionResult Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return View(); // Nếu không có query, chỉ hiển thị danh sách sản phẩm mặc định
            }

            var adminuser = _context.AdminUsers
                                    .Where(p => p.Name.Contains(query))
                                    .ToList();

            return View("Index", adminuser); // Trả về view Index với danh sách sản phẩm
        }
        // GET: Admin/AdminUser/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adminUser = await _context.AdminUsers
                .FirstOrDefaultAsync(m => m.USE_ID == id);
            if (adminUser == null)
            {
                return NotFound();
            }

            return View(adminUser);
        }

        // GET: Admin/AdminUser/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/AdminUser/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] AdminUser adminUser)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem Username đã tồn tại trong cơ sở dữ liệu chưa
                var existingUser = await _context.AdminUsers
                    .Where(u => u.Name == adminUser.Name)
                    .FirstOrDefaultAsync();

                if (existingUser != null)
                {
                    // Nếu Username đã tồn tại, thêm lỗi vào ModelState
                    ModelState.AddModelError("Username", "Username đã tồn tại. Vui lòng chọn một Username khác.");
                    return View(adminUser); // Trả về View với lỗi
                }

                // Lấy thông tin người dùng hiện tại từ session
                var userInfo = HttpContext.Session.Get<AdminUser>("userInfo");
                if (userInfo != null)
                {
                    adminUser.CreatedBy = userInfo.Name;
                    adminUser.UpdatedBy = userInfo.Name;
                }

                // Thêm AdminUser vào cơ sở dữ liệu
                _context.Add(adminUser);
                await _context.SaveChangesAsync();

                // Chuyển hướng về trang Index sau khi lưu thành công
                return RedirectToAction(nameof(Index));
            }

            // Nếu ModelState không hợp lệ, trả về View với thông tin hiện tại
            return View(adminUser);
        }



        // GET: Admin/AdminUser/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adminUser = await _context.AdminUsers.FindAsync(id);
            if (adminUser == null)
            {
                return NotFound();
            }
            return View(adminUser);
        }

        // POST: Admin/AdminUser/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] AdminUser adminUser)
        {
            if (id != adminUser.USE_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy bản ghi từ cơ sở dữ liệu
                    var existingAdminUser = await _context.AdminUsers.FindAsync(id);
                    if (existingAdminUser == null)
                    {
                        return NotFound();
                    }

                    // Kiểm tra xem Username mới có trùng với Username của người dùng khác không
                    var existingUserWithSameUsername = await _context.AdminUsers
                        .Where(u => u.Name == adminUser.Name && u.USE_ID != adminUser.USE_ID)
                        .FirstOrDefaultAsync();

                    if (existingUserWithSameUsername != null)
                    {
                        // Nếu có, thêm lỗi vào ModelState
                        ModelState.AddModelError("Username", "Username đã tồn tại. Vui lòng chọn một Username khác.");
                        return View(adminUser); // Trả về lại view với lỗi
                    }

                    // Cập nhật các trường cần thiết
                    existingAdminUser.Name = adminUser.Name;
                    existingAdminUser.Password = adminUser.Password;
                    existingAdminUser.DisplayName = adminUser.DisplayName;
                    existingAdminUser.Email = adminUser.Email;
                    existingAdminUser.Phone = adminUser.Phone;

                    // Ghi nhận người chỉnh sửa và thời gian chỉnh sửa
                    var userInfo = HttpContext.Session.Get<AdminUser>("userInfo");
                    if (userInfo != null)
                    {
                        existingAdminUser.UpdatedBy = userInfo.Name;
                    }
                    existingAdminUser.UpdatedDate = DateTime.Now;

                    // Lưu thay đổi
                    _context.Update(existingAdminUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminUserExists(adminUser.USE_ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // Nếu ModelState không hợp lệ, trả về lại view với thông tin hiện tại
            return View(adminUser);
        }



        // GET: Admin/AdminUser/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adminUser = await _context.AdminUsers
                .FirstOrDefaultAsync(m => m.USE_ID == id);
            if (adminUser == null)
            {
                return NotFound();
            }

            return View(adminUser);
        }

        // POST: Admin/AdminUser/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var adminUser = await _context.AdminUsers.FindAsync(id);
            if (adminUser != null)
            {
                _context.AdminUsers.Remove(adminUser);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdminUserExists(int id)
        {
            return _context.AdminUsers.Any(e => e.USE_ID == id);
        }

        public async Task<IActionResult> Profile()
        {
            // Lấy thông tin user từ session
            var userInfo = HttpContext.Session.Get<AdminUser>("userInfo");
            if (userInfo == null)
            {
                return RedirectToAction("Login", "Auth"); // Redirect đến trang đăng nhập nếu user chưa đăng nhập
            }

            // Lấy thông tin chi tiết của user từ database (nếu cần)
            var adminUser = await _context.AdminUsers.FirstOrDefaultAsync(u => u.USE_ID == userInfo.USE_ID);
            if (adminUser == null)
            {
                return NotFound();
            }

            return View(adminUser);
        }

        public IActionResult Logout()
        {
            // Xóa session
            HttpContext.Session.Clear();

            // Xóa cookie chứa session ID
            if (Request.Cookies.ContainsKey(".AspNetCore.Session"))
            {
                Response.Cookies.Delete(".AspNetCore.Session");
            }

            // Xóa cookie chứa thông tin đăng nhập "UserCrential"
            if (Request.Cookies.ContainsKey("UserCrential"))
            {
                Response.Cookies.Delete("UserCrential");
            }

            // Điều hướng về trang Login
            return RedirectToAction("Index", "Login");
        }



    }
}

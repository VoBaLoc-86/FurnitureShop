using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FurnitureShop.Models;
using FurnitureShop.Utils;

namespace FurnitureShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SettingController : Controller
    {
        private readonly FurnitureShopContext _context;

        public SettingController(FurnitureShopContext context)
        {
            _context = context;
        }

        // GET: Admin/Setting
        public async Task<IActionResult> Index()
        {
            return View(await _context.Settings.ToListAsync());
        }
        public IActionResult Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return View(); // Nếu không có query, chỉ hiển thị danh sách sản phẩm mặc định
            }

            var setting = _context.Settings
                                    .Where(p => p.Name.Contains(query))
                                    .ToList();

            return View("Index", setting); // Trả về view Index với danh sách sản phẩm
        }
        // GET: Admin/Setting/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var setting = await _context.Settings
                .FirstOrDefaultAsync(m => m.SET_ID == id);
            if (setting == null)
            {
                return NotFound();
            }

            return View(setting);
        }

        // GET: Admin/Setting/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Setting/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] Setting setting)
        {
            // Kiểm tra tính hợp lệ của model
            if (!ModelState.IsValid)
            {
                return View(setting);
            }

            // Kiểm tra Name đã tồn tại
            var existingSetting = await _context.Settings.FirstOrDefaultAsync(s => s.Name == setting.Name);
            if (existingSetting != null)
            {
                // Thêm lỗi vào ModelState để hiển thị thông báo lỗi
                ModelState.AddModelError("Name", "The name already exists. Please choose a different name.");
                return View(setting); // Trả lại view với thông báo lỗi
            }

            // Gắn thông tin người tạo/cập nhật
            var userInfo = HttpContext.Session.Get<AdminUser>("userInfo");
            if (userInfo != null)
            {
                setting.CreatedBy = setting.UpdatedBy = userInfo.Name;
            }

            // Thêm setting mới vào database
            _context.Add(setting);
            await _context.SaveChangesAsync();

            // Chuyển hướng đến Index sau khi lưu thành công
            return RedirectToAction(nameof(Index));
        }


        // GET: Admin/Setting/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var setting = await _context.Settings.FindAsync(id);
            if (setting == null)
            {
                return NotFound();
            }
            return View(setting);
        }

        // POST: Admin/Setting/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] Setting setting)
        {
            // Kiểm tra xem ID có khớp với bản ghi cần chỉnh sửa không
            if (id != setting.SET_ID)
            {
                return NotFound();
            }

            // Kiểm tra ModelState
            if (!ModelState.IsValid)
            {
                return View(setting);
            }

            try
            {
                // Kiểm tra Name có bị trùng với bản ghi khác không
                var duplicateSetting = await _context.Settings
                    .Where(s => s.SET_ID != id && s.Name == setting.Name)
                    .FirstOrDefaultAsync();

                if (duplicateSetting != null)
                {
                    // Thêm lỗi vào ModelState nếu Name đã tồn tại
                    ModelState.AddModelError("Name", "The name already exists. Please choose a different name.");
                    return View(setting);
                }

                // Lấy bản ghi từ cơ sở dữ liệu
                var existingSetting = await _context.Settings.FindAsync(id);
                if (existingSetting == null)
                {
                    return NotFound();
                }

                // Cập nhật các trường cần thiết
                existingSetting.Name = setting.Name;
                existingSetting.Value = setting.Value;

                // Ghi nhận người chỉnh sửa và thời gian chỉnh sửa
                var userInfo = HttpContext.Session.Get<AdminUser>("userInfo");
                if (userInfo != null)
                {
                    existingSetting.UpdatedBy = userInfo.Name;
                }
                existingSetting.UpdatedDate = DateTime.Now;

                // Lưu thay đổi vào cơ sở dữ liệu
                _context.Update(existingSetting);
                await _context.SaveChangesAsync();

                // Chuyển hướng về trang Index sau khi lưu thành công
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                // Kiểm tra xem bản ghi có tồn tại không
                if (!SettingExists(setting.SET_ID))
                {
                    return NotFound();
                }
                else
                {
                    throw; // Ném ngoại lệ nếu có lỗi đồng bộ
                }
            }
        }


        // GET: Admin/Setting/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var setting = await _context.Settings
                .FirstOrDefaultAsync(m => m.SET_ID == id);
            if (setting == null)
            {
                return NotFound();
            }

            return View(setting);
        }

        // POST: Admin/Setting/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var setting = await _context.Settings.FindAsync(id);

            // Kiểm tra xem Setting có tồn tại hay không
            if (setting != null)
            {
                _context.Settings.Remove(setting);
                await _context.SaveChangesAsync();

                // Thêm thông báo thành công vào TempData
                TempData["SuccessMessage"] = "Setting has been successfully deleted!";
            }
            else
            {
                // Nếu không tìm thấy setting, thêm thông báo lỗi vào TempData
                TempData["ErrorMessage"] = "Setting not found!";
            }

            // Sau khi xóa, chuyển hướng về trang Index
            return RedirectToAction(nameof(Index));
        }


        private bool SettingExists(int id)
        {
            return _context.Settings.Any(e => e.SET_ID == id);
        }
    }
}

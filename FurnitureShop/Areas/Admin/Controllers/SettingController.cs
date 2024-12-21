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
            if (ModelState.IsValid)
            {
                var userInfo = HttpContext.Session.Get<AdminUser>("userInfo");
                if (userInfo != null)
                {
                    setting.CreatedBy = setting.UpdatedBy = userInfo.Username;
                }
                _context.Add(setting);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(setting);
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
            if (id != setting.SET_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy bản ghi từ cơ sở dữ liệu
                    var existingSetting = await _context.Settings.FindAsync(id);
                    if (existingSetting == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật các trường cần thiết
                    existingSetting.Name = setting.Name; // Giả sử `Setting` có trường `Value`
                    existingSetting.Value = setting.Value; // Thêm các trường khác nếu cần

                    // Ghi nhận người chỉnh sửa và thời gian chỉnh sửa
                    var userInfo = HttpContext.Session.Get<AdminUser>("userInfo");
                    if (userInfo != null)
                    {
                        existingSetting.UpdatedBy = userInfo.Username;
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
                        // Ném ngoại lệ nếu xảy ra vấn đề đồng bộ
                        throw;
                    }
                }
            }

            // Nếu ModelState không hợp lệ, trả về lại view với thông tin hiện tại
            return View(setting);
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
            if (setting != null)
            {
                _context.Settings.Remove(setting);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SettingExists(int id)
        {
            return _context.Settings.Any(e => e.SET_ID == id);
        }
    }
}

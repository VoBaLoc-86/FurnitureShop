using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FurnitureShop.Models;
using FurnitureShop.Utils;
using FurnitureShop.Areas.Admin.DTOs.request;
using System.Reflection;

namespace FurnitureShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BannerController : Controller
    {
        private readonly FurnitureShopContext _context;
        private readonly IWebHostEnvironment _hostEnv;

        public BannerController(FurnitureShopContext context, IWebHostEnvironment hostEnv)
        {
            _context = context;
            _hostEnv = hostEnv;
        }

        // GET: Admin/Banner
        public async Task<IActionResult> Index()
        {
            return View(await _context.Banners.ToListAsync());
        }

        public IActionResult Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return View(); // Nếu không có query, chỉ hiển thị danh sách sản phẩm mặc định
            }

            var banner = _context.Banners
                                    .Where(p => p.Title.Contains(query))
                                    .ToList();

            return View("Index", banner); // Trả về view Index với danh sách sản phẩm
        }
        // GET: Admin/Banner/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var banner = await _context.Banners
                .FirstOrDefaultAsync(m => m.BAN_ID == id);
            if (banner == null)
            {
                return NotFound();
            }

            return View(banner);
        }

        // GET: Admin/Banner/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Banner/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] BannerDTO request)
        {
            var banner =new Banner() { 
                Title = request.Title,
                DisplayOrder = request.DisplayOrder,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };
            var userInfo = HttpContext.Session.Get<AdminUser>("userInfo");
            if (userInfo != null)
            {
                banner.CreatedBy = userInfo.Username;
                banner.UpdatedBy = userInfo.Username;
            }
            if (ModelState.IsValid)
            {
                string? newImageFileName = null;
                if (request.Image != null)
                {
                    var extension = Path.GetExtension(request.Image.FileName);
                    newImageFileName = $"{Guid.NewGuid().ToString()} {extension}";
                    var filePath = Path.Combine(_hostEnv.WebRootPath, "data", "banners", newImageFileName);
                    request.Image.CopyTo(new FileStream(filePath, FileMode.Create));
                }
                if (newImageFileName != null) { banner.Image = newImageFileName; }
                _context.Add(banner);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(banner);
        }

        // GET: Admin/Banner/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var banner = await _context.Banners.FindAsync(id);
            if (banner == null)
            {
                return NotFound();
            }
            return View(banner);
        }

        // POST: Admin/Banner/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] BannerDTO request)
        {
            if (id != request.BAN_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy bản ghi từ cơ sở dữ liệu
                    var existingBanner = await _context.Banners.FindAsync(id);
                    if (existingBanner == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật các trường cần thiết
                    existingBanner.Title = request.Title;
                    existingBanner.DisplayOrder = request.DisplayOrder;

                    // Ghi nhận người chỉnh sửa và thời gian chỉnh sửa
                    var userInfo = HttpContext.Session.Get<AdminUser>("userInfo");
                    if (userInfo != null)
                    {
                        existingBanner.UpdatedBy = userInfo.Username;
                    }
                    existingBanner.UpdatedDate = DateTime.Now; // Cập nhật thời gian sửa đổi

                    // Xử lý upload ảnh (nếu có)
                    if (request.Image != null)
                    {
                        string? newImageFileName = null;
                        var extension = Path.GetExtension(request.Image.FileName);
                        newImageFileName = $"{Guid.NewGuid()}{extension}";
                        var filePath = Path.Combine(_hostEnv.WebRootPath, "data", "banners", newImageFileName);

                        // Lưu file mới
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await request.Image.CopyToAsync(stream);
                        }

                        // Xóa ảnh cũ (nếu cần thiết)
                        if (!string.IsNullOrEmpty(existingBanner.Image))
                        {
                            var oldFilePath = Path.Combine(_hostEnv.WebRootPath, "data", "banners", existingBanner.Image);
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        // Gán tên file mới cho banner
                        existingBanner.Image = newImageFileName;
                    }

                    // Lưu thay đổi vào cơ sở dữ liệu
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Kiểm tra xem bản ghi có tồn tại không
                    if (!BannerExists(request.BAN_ID))
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

            return View(request);
        }

        // GET: Admin/Banner/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var banner = await _context.Banners
                .FirstOrDefaultAsync(m => m.BAN_ID == id);
            if (banner == null)
            {
                return NotFound();
            }

            return View(banner);
        }

        // POST: Admin/Banner/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var banner = await _context.Banners.FindAsync(id);
            if (banner != null)
            {
                _context.Banners.Remove(banner);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BannerExists(int id)
        {
            return _context.Banners.Any(e => e.BAN_ID == id);
        }
    }
}

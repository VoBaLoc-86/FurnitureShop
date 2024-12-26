using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FurnitureShop.Models;
using Azure.Core;
using FurnitureShop.Areas.Admin.DTOs.request;
using FurnitureShop.Utils;

namespace FurnitureShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PageController : Controller
    {
        private readonly FurnitureShopContext _context;
        private readonly IWebHostEnvironment _hostEnv;

        public PageController(IWebHostEnvironment hostEnv, FurnitureShopContext context)
        {
            _context = context;
            _hostEnv = hostEnv;
        }

        

        // GET: Admin/Page
        public async Task<IActionResult> Index()
        {
            return View(await _context.Pages.ToListAsync());
        }

        // GET: Admin/Page/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var page = await _context.Pages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (page == null)
            {
                return NotFound();
            }

            return View(page);
        }

        // GET: Admin/Page/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Page/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] PageDTO request)
        {
            if (!ModelState.IsValid)
            {
                
                return View(request); // Quay lại trang Create và giữ các giá trị đã nhập
            }

            // Khởi tạo đối tượng product từ request
            var page = new Page()
            {
                Title = request.Title,
                Content = request.Content,
                DisplayOrder = request.DisplayOrder,
            };

            // Ghi nhận thông tin người tạo từ session
            var userInfo = HttpContext.Session.Get<AdminUser>("userInfo");
            if (userInfo != null)
            {
                page.CreatedBy = page.UpdatedBy = userInfo.Name;
            }

            // Ghi nhận thời gian tạo và cập nhật
            page.CreatedDate = page.UpdatedDate = DateTime.Now;

            // Xử lý ảnh tải lên
            string? newImageFileName = null;
            if (request.Image != null)
            {
                var extension = Path.GetExtension(request.Image.FileName);
                newImageFileName = $"{Guid.NewGuid().ToString()}{extension}";
                var filePath = Path.Combine(_hostEnv.WebRootPath, "data", "pages", newImageFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.Image.CopyToAsync(stream);
                }

                // Gán tên file ảnh vào sản phẩm nếu có
                page.Image = newImageFileName;
            }

            // Thêm sản phẩm vào cơ sở dữ liệu
            _context.Add(page);
            await _context.SaveChangesAsync();

            // Chuyển hướng về trang Index
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Page/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var page = await _context.Pages.FindAsync(id);
            if (page == null)
            {
                return NotFound();
            }
            return View(page);
        }

        // POST: Admin/Page/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] PageDTO update)
        {
            if (id != update.Id)
            {
                return NotFound();
            }
            string Username = "";
            if (ModelState.IsValid)
            {
                var userInfo = HttpContext.Session.Get<AdminUser>("userInfo");
                if (userInfo != null)
                {
                    Username = userInfo.Name;
                }
                try
                {
                    var existingPage = await _context.Pages.FindAsync(id);
                    if (existingPage == null)
                    {
                        return NotFound();
                    }

                    // Nếu có ảnh mới được chọn, xử lý lưu ảnh mới
                    if (update.Image != null)
                    {
                        string? newImageFileName = null;
                        var extension = Path.GetExtension(update.Image.FileName);
                        newImageFileName = $"{Guid.NewGuid()}{extension}";
                        var filePath = Path.Combine(_hostEnv.WebRootPath, "data", "pages", newImageFileName);

                        // Lưu file mới
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await update.Image.CopyToAsync(stream);
                        }

                        // Xóa ảnh cũ (nếu cần thiết)
                        if (!string.IsNullOrEmpty(existingPage.Image))
                        {
                            var oldFilePath = Path.Combine(_hostEnv.WebRootPath, "data", "pages", existingPage.Image);
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        // Gán tên file mới cho sản phẩm
                        existingPage.Image = newImageFileName;
                    }
                    else
                    {
                        // Nếu không có ảnh mới, giữ lại ảnh cũ
                        existingPage.Image = existingPage.Image;
                    }

                    // Kiểm tra và cập nhật các giá trị còn lại
                    existingPage.Title = update.Title;
                    existingPage.Content = update.Content;
                    existingPage.DisplayOrder = update.DisplayOrder;

                    // Cập nhật trang
                    _context.Update(existingPage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PageExists(update.Id))
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
            return View(update);
        }


        // GET: Admin/Page/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var page = await _context.Pages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (page == null)
            {
                return NotFound();
            }

            return View(page);
        }

        // POST: Admin/Page/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var page = await _context.Pages.FindAsync(id);
            if (page != null)
            {
                _context.Pages.Remove(page);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PageExists(int id)
        {
            return _context.Pages.Any(e => e.Id == id);
        }
    }
}

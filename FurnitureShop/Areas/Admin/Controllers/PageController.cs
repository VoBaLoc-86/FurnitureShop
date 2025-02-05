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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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


        public IActionResult Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return View(); // Nếu không có query, chỉ hiển thị danh sách sản phẩm mặc định
            }

            var page = _context.Pages
                                    .Where(p => p.Title.Contains(query))
                                    .ToList();

            return View("Index", page); // Trả về view Index với danh sách sản phẩm
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

            if (request.Image == null)
            {
                TempData["ErrorMessage"] = "Hình ảnh là bắt buộc. Vui lòng tải lên một ảnh.";
                return View();
            }

            // Kiểm tra nếu title đã tồn tại
            var existingPage = await _context.Pages
                                              .FirstOrDefaultAsync(p => p.Title.ToLower() == request.Title.ToLower());
            if (existingPage != null)
            {
                TempData["ErrorMessage"] = "Title đã tồn tại. Vui lòng nhập tên khác.";
                return View(request); // Quay lại trang Create và giữ các giá trị đã nhập
            }

            // Khởi tạo đối tượng page từ request
            var page = new Page()
            {
                Title = request.Title,
                Content = request.Content,
                DisplayOrder = request.DisplayOrder,
            };

            // Ghi nhận thông tin người tạo từ session
            var userInfo = HttpContext.Session.Get<AdminUser>("adminInfo");
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

            // Thêm page vào cơ sở dữ liệu
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

            if (ModelState.IsValid)
            {
                try
                {
                    var existingPage = await _context.Pages.FindAsync(id);
                    if (existingPage == null)
                    {
                        return NotFound();
                    }

                    // Kiểm tra nếu Title đã tồn tại
                    var duplicatePage = await _context.Pages
                        .Where(p => p.Id != id && p.Title.ToLower() == update.Title.ToLower())
                        .FirstOrDefaultAsync();

                    if (duplicatePage != null)
                    {
                        ModelState.AddModelError("Title", "Title đã tồn tại. Vui lòng nhập tên khác.");
                        // Chuyển đổi từ PageDTO sang Page trước khi trả về view
                        var model = new Page
                        {
                            Id = update.Id,
                            Title = update.Title,
                            Content = update.Content,
                            DisplayOrder = update.DisplayOrder,
                            Image = existingPage.Image // Giữ nguyên ảnh hiện tại
                        };
                        return View(model);
                    }

                    // Xử lý cập nhật nếu không có lỗi
                    if (update.Image != null)
                    {
                        string newImageFileName = $"{Guid.NewGuid()}{Path.GetExtension(update.Image.FileName)}";
                        var filePath = Path.Combine(_hostEnv.WebRootPath, "data", "pages", newImageFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await update.Image.CopyToAsync(stream);
                        }

                        if (!string.IsNullOrEmpty(existingPage.Image))
                        {
                            var oldFilePath = Path.Combine(_hostEnv.WebRootPath, "data", "pages", existingPage.Image);
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        existingPage.Image = newImageFileName;
                    }

                    existingPage.Title = update.Title;
                    existingPage.Content = update.Content;
                    existingPage.DisplayOrder = update.DisplayOrder;

                    _context.Update(existingPage);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
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
            }

            // Chuyển đổi từ PageDTO sang Page nếu ModelState không hợp lệ
            var modelFallback = new Page
            {
                Id = update.Id,
                Title = update.Title,
                Content = update.Content,
                DisplayOrder = update.DisplayOrder,
                Image = update.Image?.FileName // Giữ thông tin ảnh mới (nếu có)
            };
            return View(modelFallback);
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
                TempData["SuccessMessage"] = "Page deleted successfully."; // Lưu thông báo thành công vào TempData
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

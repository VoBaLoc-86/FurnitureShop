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
using Azure.Core;

namespace FurnitureShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FeatureController : Controller
    {
        private readonly IWebHostEnvironment _hostEnv;
        private readonly FurnitureShopContext _context;
        public FeatureController(IWebHostEnvironment hostEnv, FurnitureShopContext context)
        {
            _context = context;
            _hostEnv = hostEnv;
        }

        // GET: Admin/Feature
        public async Task<IActionResult> Index()
        {
            return View(await _context.Features.ToListAsync());
        }

        // GET: Admin/Feature/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feature = await _context.Features
                .FirstOrDefaultAsync(m => m.FEA_ID == id);
            if (feature == null)
            {
                return NotFound();
            }

            return View(feature);
        }

        // GET: Admin/Feature/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Feature/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] FeatureDTO request)
        {
            var feature = new Feature()
            {
                Icon = "",
                Title = request.Title,
                Subtitle = request.Subtitle,

            };


            if (ModelState.IsValid)
            {
                var userInfo = HttpContext.Session.Get<AdminUser>("userInfo");
                if (userInfo != null)
                {
                    feature.CreatedBy = feature.UpdatedBy = userInfo.Username ;
                }
                string? newImageFileName = null;
                if (request.Icon != null)
                {
                    var extension = Path.GetExtension(request.Icon.FileName);
                    newImageFileName = $"{Guid.NewGuid().ToString()} {extension}";
                    var filePath = Path.Combine(_hostEnv.WebRootPath, "data", "features", newImageFileName);
                    request.Icon.CopyTo(new FileStream(filePath, FileMode.Create));
                }
                if (newImageFileName != null) { feature.Icon = newImageFileName; }
                _context.Add(feature);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(feature);
        }

        // GET: Admin/Feature/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feature = await _context.Features.FindAsync(id);
            if (feature == null)
            {
                return NotFound();
            }
            return View(feature);
        }

        // POST: Admin/Feature/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] FeatureDTO update)
        {
            if (id != update.FEA_ID)
            {
                return NotFound();
            }
            string Username = "";
            if (ModelState.IsValid)
            {
                var userInfo = HttpContext.Session.Get<AdminUser>("userInfo");
                if (userInfo != null)
                {
                    Username = userInfo.Username;
                }
                try
                {
                    var existingFeature = await _context.Features.FindAsync(id);
                    if (existingFeature == null)
                    {
                        return NotFound();
                    }
                    if (update.Icon != null)
                    {
                        string? newImageFileName = null;
                        var extension = Path.GetExtension(update.Icon.FileName);
                        newImageFileName = $"{Guid.NewGuid()}{extension}";
                        var filePath = Path.Combine(_hostEnv.WebRootPath, "data", "features", newImageFileName);

                        // Lưu file mới
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await update.Icon.CopyToAsync(stream);
                        }

                        // Xóa ảnh cũ (nếu cần thiết)
                        if (!string.IsNullOrEmpty(existingFeature.Icon))
                        {
                            var oldFilePath = Path.Combine(_hostEnv.WebRootPath, "data", "features", existingFeature.Icon);
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        // Gán tên file mới cho sản phẩm
                        existingFeature.Icon = newImageFileName;
                    }
                    // Chỉ cập nhật các trường cụ thể
                    existingFeature.Title = update.Title;
                    existingFeature.Subtitle = update.Subtitle;
                    existingFeature.DisplayOrder = update.DisplayOrder;

                    // Cập nhật các trường tự động
                    existingFeature.UpdatedDate = DateTime.Now;
                    existingFeature.UpdatedBy = Username; // Hoặc giá trị khác bạn mong muốn

                    // Lưu thay đổi
                    _context.Update(existingFeature);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FeatureExists(update.FEA_ID))
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

        // GET: Admin/Feature/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feature = await _context.Features
                .FirstOrDefaultAsync(m => m.FEA_ID == id);
            if (feature == null)
            {
                return NotFound();
            }

            return View(feature);
        }

        // POST: Admin/Feature/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var feature = await _context.Features.FindAsync(id);
            if (feature != null)
            {
                _context.Features.Remove(feature);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FeatureExists(int id)
        {
            return _context.Features.Any(e => e.FEA_ID == id);
        }
    }
}

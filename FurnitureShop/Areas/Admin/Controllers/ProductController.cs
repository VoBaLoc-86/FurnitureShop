using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FurnitureShop.Models;
using FurnitureShop.Areas.Admin.DTOs.request;
using FurnitureShop.Utils;

namespace FurnitureShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly FurnitureShopContext _context;
        private readonly IWebHostEnvironment _hostEnv;

        public ProductController(IWebHostEnvironment hostEnv,FurnitureShopContext context)
        {
            _context = context;
            _hostEnv = hostEnv;
        }

        // GET: Admin/Product
        public async Task<IActionResult> Index()
        {
            // Lấy tất cả sản phẩm và bao gồm thông tin Category
            var furnitureShopContext = _context.Products.Include(p => p.Category);
            return View(await furnitureShopContext.ToListAsync());
        }

        public IActionResult Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return RedirectToAction("Index", "Product");  // Chuyển về Index của ProductController
            }

            var products = _context.Products
                                    .Where(p => p.Name.Contains(query))
                                    .Include(p => p.Category)
                                    .ToList();

            return View("Index", products);  // Trả về view Index với các sản phẩm tìm được
        }





        // GET: Admin/Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products!
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/Product/Create
        public IActionResult Create()
        {
            ViewData["Category_id"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Admin/Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] ProductDTO request)
        {
            var product = new Product()
            {
                Id = request.Id,
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Stock = request.Stock,
                Category_id = request.Category_id,
            };

            if (ModelState.IsValid)
            {
                // Ghi nhận thông tin người tạo từ session
                var userInfo = HttpContext.Session.Get<AdminUser>("userInfo");
                if (userInfo != null)
                {
                    product.CreatedBy = product.UpdatedBy = userInfo.Username;
                }

                // Ghi nhận thời gian tạo và cập nhật
                product.CreatedDate = product.UpdatedDate = DateTime.Now;

                // Xử lý ảnh tải lên
                string? newImageFileName = null;
                if (request.Image != null)
                {
                    var extension = Path.GetExtension(request.Image.FileName);
                    newImageFileName = $"{Guid.NewGuid().ToString()}{extension}";
                    var filePath = Path.Combine(_hostEnv.WebRootPath, "data", "products", newImageFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.Image.CopyToAsync(stream);
                    }
                }

                // Gán tên file ảnh vào sản phẩm nếu có
                if (newImageFileName != null)
                {
                    product.Image = newImageFileName;
                }

                // Thêm sản phẩm vào cơ sở dữ liệu
                _context.Add(product);
                await _context.SaveChangesAsync();

                // Chuyển hướng về trang Index
                return RedirectToAction(nameof(Index));
            }

            // Trả về View với danh sách danh mục nếu ModelState không hợp lệ
            ViewData["Category_id"] = new SelectList(_context.Categories, "Id", "Name", product.Category_id);
            return View(product);
        }


        // GET: Admin/Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products!.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["Category_id"] = new SelectList(_context.Categories, "Id", "Id", product.Category_id);
            return View(product);
        }

        // POST: Admin/Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] ProductDTO update)
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
                    Username = userInfo.Username;
                }
                try
                {
                    var existingProduct = await _context.Products.FindAsync(id);
                    if (existingProduct == null)
                    {
                        return NotFound();
                    }
                    if (update.Image != null)
                    {
                        string? newImageFileName = null;
                        var extension = Path.GetExtension(update.Image.FileName);
                        newImageFileName = $"{Guid.NewGuid()}{extension}";
                        var filePath = Path.Combine(_hostEnv.WebRootPath, "data", "products", newImageFileName);

                        // Lưu file mới
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await update.Image.CopyToAsync(stream);
                        }

                        // Xóa ảnh cũ (nếu cần thiết)
                        if (!string.IsNullOrEmpty(existingProduct.Image))
                        {
                            var oldFilePath = Path.Combine(_hostEnv.WebRootPath, "data", "products", existingProduct.Image);
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        // Gán tên file mới cho sản phẩm
                        existingProduct.Image = newImageFileName;
                    }
                    existingProduct.Name = update.Name;
                    existingProduct.Description = update.Description;
                    existingProduct.Price = update.Price;
                    existingProduct.Stock = update.Stock;
                    existingProduct.Category_id = update.Category_id;
                    existingProduct.UpdatedBy = Username;
                    existingProduct.UpdatedDate = DateTime.Now;


                    _context.Update(existingProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(update.Id))
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
            ViewData["Category_id"] = new SelectList(_context.Categories, "Id", "Id", update.Category_id);
            return View(update);
        }

        // GET: Admin/Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products!
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products!.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products!.Any(e => e.Id == id);
        }
    }
}

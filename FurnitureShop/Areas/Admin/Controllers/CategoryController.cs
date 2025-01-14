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
    public class CategoryController : Controller
    {
        private readonly FurnitureShopContext _context;

        public CategoryController(FurnitureShopContext context)
        {
            _context = context;
        }

        // GET: Admin/Category
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.ToListAsync();
            var categoryProductInfo = new Dictionary<int, bool>();

            foreach (var category in categories)
            {
                categoryProductInfo[category.Id] = await HasProducts(category.Id);
            }

            ViewData["CategoryProductInfo"] = categoryProductInfo;

            return View(categories);
        }



        public IActionResult Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return View(); // Nếu không có query, chỉ hiển thị danh sách sản phẩm mặc định
            }

            var category = _context.Categories
                                    .Where(p => p.Name.Contains(query))
                                    .ToList();

            return View("Index", category); // Trả về view Index với danh sách sản phẩm
        }
        // GET: Admin/Category/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Admin/Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Category/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] Category category)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem Name đã tồn tại trong cơ sở dữ liệu chưa
                var existingCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Name == category.Name);

                if (existingCategory != null)
                {
                    // Nếu Name đã tồn tại, thêm lỗi vào ModelState
                    ModelState.AddModelError("Name", "Tên danh mục đã tồn tại. Vui lòng chọn một tên khác.");
                    return View(category); // Trả về View với thông báo lỗi
                }

                // Lấy thông tin người dùng hiện tại từ session
                var userInfo = HttpContext.Session.Get<AdminUser>("adminInfo");
                if (userInfo != null)
                {
                    category.CreatedBy = category.UpdatedBy = userInfo.Name;
                }

                // Thêm Category vào cơ sở dữ liệu
                _context.Add(category);
                await _context.SaveChangesAsync();

                // Chuyển hướng về trang Index sau khi lưu thành công
                return RedirectToAction(nameof(Index));
            }

            // Nếu ModelState không hợp lệ, trả về View với thông tin hiện tại
            return View(category);
        }


        // GET: Admin/Category/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Admin/Category/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy bản ghi từ cơ sở dữ liệu
                    var existingCategory = await _context.Categories.FindAsync(id);
                    if (existingCategory == null)
                    {
                        return NotFound();
                    }

                    // Kiểm tra xem Name có trùng với danh mục khác không
                    var existingCategoryWithSameName = await _context.Categories
                        .Where(c => c.Name == category.Name && c.Id != category.Id)
                        .FirstOrDefaultAsync();

                    if (existingCategoryWithSameName != null)
                    {
                        // Nếu Name trùng, thêm lỗi vào ModelState
                        ModelState.AddModelError("Name", "Tên danh mục đã tồn tại. Vui lòng chọn một tên khác.");
                        return View(category); // Trả về lại view với lỗi
                    }

                    // Cập nhật các trường cần thiết
                    existingCategory.Name = category.Name;

                    // Ghi nhận người chỉnh sửa và thời gian chỉnh sửa
                    var userInfo = HttpContext.Session.Get<AdminUser>("adminInfo");
                    if (userInfo != null)
                    {
                        existingCategory.UpdatedBy = userInfo.Name;
                    }
                    existingCategory.UpdatedDate = DateTime.Now;

                    // Lưu thay đổi vào cơ sở dữ liệu
                    _context.Update(existingCategory);
                    await _context.SaveChangesAsync();

                    // Chuyển hướng về trang Index sau khi lưu thành công
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Kiểm tra xem bản ghi có tồn tại không
                    if (!CategoryExists(category.Id))
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
            return View(category);
        }


        // GET: Admin/Category/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Admin/Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories
                                         .Include(c => c.Products) // Load danh sách Products liên quan
                                         .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                TempData["ErrorMessage"] = "Category not found.";
                return View(category);  // Trả về view Delete của category với thông báo lỗi
            }

            if (category.Products != null && category.Products.Any())
            {
                // Thông báo nếu không thể xóa vì có sản phẩm liên quan
                TempData["ErrorMessage"] = "Cannot delete this category because it has associated products.";
                return View(category);  // Trả về view Delete của category với thông báo lỗi
            }

            try
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Category deleted successfully."; // Lưu thông báo thành công vào TempData
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error occurred while deleting category: {ex.Message}";
            }

            return RedirectToAction(nameof(Index)); // Chuyển hướng về Index sau khi xóa thành công
        }

        public async Task<bool> HasProducts(int categoryId)
        {
            var category = await _context.Categories
                                          .Include(c => c.Products)
                                          .FirstOrDefaultAsync(c => c.Id == categoryId);

            return category != null && category.Products.Any();
        }




        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}

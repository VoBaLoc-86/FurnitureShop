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
    public class ReviewController : Controller
    {
        private readonly FurnitureShopContext _context;

        public ReviewController(FurnitureShopContext context)
        {
            _context = context;
        }

        // GET: Admin/Review
        public async Task<IActionResult> Index()
        {
            var furnitureShopContext = _context.Reviews!.Include(r => r.Product).Include(r => r.User);
            return View(await furnitureShopContext.ToListAsync());
        }

        public IActionResult Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return View(); // Nếu không có query, chỉ hiển thị danh sách sản phẩm mặc định
            }

            var review = _context.Reviews
                                    .Where(p => p.User!.Name.Contains(query))
                                    .ToList();

            return View("Index", review); // Trả về view Index với danh sách sản phẩm
        }
        // GET: Admin/Review/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews!
                .Include(r => r.Product)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // GET: Admin/Review/Create
        public IActionResult Create()
        {
            ViewData["Product_id"] = new SelectList(_context.Products, "Id", "Name");
            ViewData["User_id"] = new SelectList(_context.Users, "Id", "Name");
            return View();
        }

        // POST: Admin/Review/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] Review review)
        {
            if (ModelState.IsValid)
            {
                var userInfo = HttpContext.Session.Get<AdminUser>("userInfo");
                if (userInfo != null)
                {
                    review.CreatedBy = review.UpdatedBy = userInfo.Username;
                }
                _context.Add(review);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Product_id"] = new SelectList(_context.Products, "Id", "Name", review.Product_id);
            ViewData["User_id"] = new SelectList(_context.Users, "Id", "Name", review.User_id);
            return View(review);
        }

        // GET: Admin/Review/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews!.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }
            ViewData["Product_id"] = new SelectList(_context.Products, "Id", "Name", review.Product_id);
            ViewData["User_id"] = new SelectList(_context.Users, "Id", "Name", review.User_id);
            return View(review);
        }

        // POST: Admin/Review/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] Review review)
        {
            if (id != review.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingReview = await _context.Reviews.FindAsync(id);
                    if (existingReview == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật các trường cần thiết
                    existingReview.Product_id = review.Product_id;
                    existingReview.User_id = review.User_id;
                    existingReview.Rating = review.Rating;
                    existingReview.Comment = review.Comment;



                    // Ghi nhận người chỉnh sửa và thời gian chỉnh sửa
                    var userInfo = HttpContext.Session.Get<AdminUser>("userInfo");
                    if (userInfo != null)
                    {
                        existingReview.UpdatedBy = userInfo.Username;
                    }
                    existingReview.UpdatedDate = DateTime.Now;

                    // Lưu thay đổi
                    _context.Update(existingReview);
                    await _context.SaveChangesAsync();



                    _context.Update(review);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewExists(review.Id))
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
            ViewData["Product_id"] = new SelectList(_context.Products, "Id", "Name", review.Product_id);
            ViewData["User_id"] = new SelectList(_context.Users, "Id", "Name", review.User_id);
            return View(review);
        }

        // GET: Admin/Review/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews!
                .Include(r => r.Product)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // POST: Admin/Review/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.Reviews!.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();

                // Thêm thông báo xóa thành công vào TempData
                TempData["SuccessMessage"] = "Review deleted successfully.";
            }
            else
            {
                // Thông báo lỗi nếu không tìm thấy review
                TempData["ErrorMessage"] = "Review not found.";
            }

            // Chuyển hướng lại trang Index
            return RedirectToAction(nameof(Index));
        }


        private bool ReviewExists(int id)
        {
            return _context.Reviews!.Any(e => e.Id == id);
        }
    }
}

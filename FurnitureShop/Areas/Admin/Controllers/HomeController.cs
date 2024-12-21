using FurnitureShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurnitureShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly FurnitureShopContext _context;

        public HomeController(FurnitureShopContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            ViewData["Products"] = await _context.Products!.ToListAsync();
            ViewData["Categories"] = await _context.Categories!.ToListAsync();
            ViewData["Reviews"] = await _context.Reviews!.ToListAsync();
            ViewData["Users"] = await _context.Users!.ToListAsync();
            ViewData["Orders"] = await _context.Orders!.ToListAsync();
            ViewData["Banners"] = await _context.Banners!.ToListAsync();
            ViewData["Features"] = await _context.Features!.ToListAsync();
            ViewData["AdminUsers"] = await _context.AdminUsers!.ToListAsync();
            ViewData["Pages"] = await _context.Pages!.ToListAsync();
            ViewData["Settings"] = await _context.Settings!.ToListAsync();


            return View();
        }
        [HttpGet]
        public IActionResult Search(string query, string category)
        {
            if (string.IsNullOrEmpty(query) || string.IsNullOrEmpty(category))
            {
                return BadRequest("Query and category are required.");
            }

            switch (category)
            {
                case "Products":
                    return RedirectToAction("Search", "Product", new { query = query });
                case "Categories":
                    return RedirectToAction("Search", "Category", new { query = query });

                case "Reviews":
                    return RedirectToAction("Search", "Review", new { query = query });

                case "Users":
                    return RedirectToAction("Search", "User", new { query = query });

                case "Banners":
                    return RedirectToAction("Search", "Banner", new { query = query });

                case "AdminUsers":
                    return RedirectToAction("Search", "AdminUser", new { query = query });

                case "Orders":
                    return RedirectToAction("Search", "Order", new { query = query });

                case "Settings":
                    return RedirectToAction("Search", "Setting", new { query = query });

                case "Pages":
                    return RedirectToAction("Search", "Page", new { query = query });
                default:
                    return NotFound("Category not supported.");
            }
        }

    }
}

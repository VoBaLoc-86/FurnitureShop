using FurnitureShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurnitureShop.Controllers
{
    public class ShopController : Controller
    {
        private readonly FurnitureShopContext _context;
        public ShopController(FurnitureShopContext context)
        {
            _context = context;
        }
        public IActionResult Index(string? search, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.Products.AsNoTracking().AsQueryable();

            // Search by product name
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Name.Contains(search));
                ViewData["SearchQuery"] = search;
            }

            // Filter by price range
            if (minPrice.HasValue)
            {
                query = query.Where(x => x.Price >= minPrice.Value);
                ViewData["MinPrice"] = minPrice.Value;
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(x => x.Price <= maxPrice.Value);
                ViewData["MaxPrice"] = maxPrice.Value;
            }

            // Return filtered or default products
            ViewData["Products"] = query.ToList();
            ViewData["PageShop"] = _context.Pages!.AsNoTracking().FirstOrDefault(x => x.Title == "Shop");
            

            return View();
        }



        public async Task<IActionResult> Details(long Id)
        {
            var product = await _context.Products
                            .AsNoTracking()
                            .Include(p => p.Category)
                            .FirstOrDefaultAsync(p => p.Id == Id);
            ViewData["categories"] = await _context.Categories
                                    .AsNoTracking()
                                    .Include(c => c.Products)
                                    .ToListAsync();
            ViewData["relatedproducts"] = await _context.Products
                                            .Include(p => p.Category)
                                            .Where(p => p.Category == product!.Category && p.Id != product.Id)
                                            .ToListAsync();
            return View(product);
        }
    }
}

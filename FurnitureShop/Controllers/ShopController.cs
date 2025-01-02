using FurnitureShop.Models;
using FurnitureShop.Utils;
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
                            .Include(p=>p.Reviews).ThenInclude(r => r.User)
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
        public IActionResult AddToCart(int productId, int quantity)
        {
            // Lấy thông tin sản phẩm từ cơ sở dữ liệu (Model)
            var product = _context.Products.FirstOrDefault(p => p.Id == productId);

            if (product == null)
            {
                return NotFound();
            }

            // Lấy giỏ hàng từ Session
            var cart = HttpContext.Session.Get<List<CartItem>>("cart") ?? new List<CartItem>();

            // Kiểm tra xem sản phẩm đã có trong giỏ hàng chưa
            var existingItem = cart.FirstOrDefault(item => item.Id == productId);

            if (existingItem != null)
            {
                // Nếu sản phẩm đã có, cập nhật số lượng
                existingItem.Quantity += quantity;
            }
            else
            {
                // Nếu chưa có, thêm sản phẩm vào giỏ hàng
                cart.Add(new CartItem
                {
                    Id = productId,
                    Name = product.Name,
                    Price = product.Price,
                    Quantity = quantity,
                    Image = product.Image
                });
            }

            // Lưu giỏ hàng vào Session
            HttpContext.Session.Set("cart", cart);

            // Chuyển hướng về trang giỏ hàng hoặc thông báo thành công
            return RedirectToAction("Index", "Cart");
        }
    }
}

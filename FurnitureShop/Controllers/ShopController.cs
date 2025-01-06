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



        public async Task<IActionResult> Details(string productName)
        {
            // Tải toàn bộ sản phẩm từ cơ sở dữ liệu
            var products = await _context.Products
                             .AsNoTracking()
                             .Include(p => p.Category)
                             .Include(p => p.Reviews).ThenInclude(r => r.User)
                             .ToListAsync();

            // Tìm sản phẩm bằng URL thân thiện
            var product = products.FirstOrDefault(p => CreateNameUrl.CreateProductUrl(p.Name) == productName);

            if (product == null)
            {
                return NotFound();
            }

            ViewData["categories"] = await _context.Categories
                                    .AsNoTracking()
                                    .Include(c => c.Products)
                                    .ToListAsync();
            ViewData["relatedproducts"] = products
                                            .Where(p => p.Category == product!.Category && p.Id != product.Id)
                                            .ToList();

            return View(product);
        }
        public IActionResult AddToCart(string productName, int quantity)
        {
            // Tải toàn bộ sản phẩm từ cơ sở dữ liệu
            var products = _context.Products.ToList();

            // Tìm sản phẩm bằng URL thân thiện
            var product = products.FirstOrDefault(p => CreateNameUrl.CreateProductUrl(p.Name) == productName);

            if (product == null)
            {
                return NotFound();
            }

            // Lấy giỏ hàng từ Session
            var cart = HttpContext.Session.Get<List<CartItem>>("cart") ?? new List<CartItem>();

            // Kiểm tra xem sản phẩm đã có trong giỏ hàng chưa
            var existingItem = cart.FirstOrDefault(item => item.Id == product.Id);

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
                    Id = product.Id,
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

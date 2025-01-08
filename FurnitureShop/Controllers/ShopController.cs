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
    .Where(p => p.Category_id == product!.Category_id && CreateNameUrl.CreateProductUrl(p.Name) != productName)
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

            // Kiểm tra nếu số lượng yêu cầu vượt quá số lượng tồn kho
            if (quantity > product.Stock)
            {
                TempData["Error"] = "The quantity exceeds available stock.";
                return RedirectToAction("Details", new { productName });
            }

            // Lấy giỏ hàng từ Session
            var cart = HttpContext.Session.Get<List<CartItem>>("cart") ?? new List<CartItem>();

            // Kiểm tra xem sản phẩm đã có trong giỏ hàng chưa
            var existingItem = cart.FirstOrDefault(item => item.Id == product.Id);

            if (existingItem != null)
            {
                // Kiểm tra nếu số lượng yêu cầu cộng thêm vào giỏ hàng vượt quá số lượng tồn kho
                if (existingItem.Quantity + quantity > product.Stock)
                {
                    TempData["Error"] = "The quantity in the cart exceeds available stock.";
                    return RedirectToAction("Details", new { productName });
                }

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
            TempData["Success"] = "Product added to cart successfully.";
            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(int productId, string content, int rating)
        {
            // Lấy thông tin người dùng từ session (toàn bộ đối tượng User)
            var user = HttpContext.Session.Get<User>("userInfo");

            if (user == null)
            {
                TempData["Error"] = "You must be logged in to leave a review.";

                // Lấy sản phẩm để chuyển đổi sang URL thân thiện
                var product = await _context.Products
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == productId);

                if (product == null)
                {
                    return NotFound();
                }

                var productName = CreateNameUrl.CreateProductUrl(product.Name);
                return RedirectToAction("Details", new { productName });
            }

            // Kiểm tra xem người dùng đã mua sản phẩm trong đơn hàng thành công hay chưa
            var hasPurchased = await _context.Orders
                .Where(o => o.User_id == user.Id && o.Status == "Completed")
                .AnyAsync(o => o.Order_Details.Any(od => od.Product_id == productId));

            if (!hasPurchased)
            {
                TempData["Error"] = "You can only review products you have purchased in a successful order.";
                return RedirectToAction("Details", new { productName = CreateNameUrl.CreateProductUrl((await _context.Products.FindAsync(productId))?.Name ?? "") });
            }

            // Tạo mới đối tượng Review
            var review = new Review
            {
                Product_id = productId,
                User_id = user.Id,       // Lấy ID từ đối tượng User
                Comment = content,
                Rating = rating,         // Lưu rating từ form
                CreatedDate = DateTime.Now,
                CreatedBy = user.Name,   // Lấy tên từ đối tượng User
            };

            // Thêm review vào cơ sở dữ liệu
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Your review has been submitted successfully.";

            // Lấy sản phẩm để tạo URL thân thiện và chuyển hướng
            var redirectProduct = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (redirectProduct == null)
            {
                return NotFound();
            }

            var redirectProductName = CreateNameUrl.CreateProductUrl(redirectProduct.Name);
            return RedirectToAction("Details", new { productName = redirectProductName });
        }




    }
}

using FurnitureShop.Models;
using FurnitureShop.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class ShopController : Controller
{
    private readonly FurnitureShopContext _context;
    public ShopController(FurnitureShopContext context)
    {
        _context = context;
    }

    public IActionResult Index(string? search, decimal? minPrice, decimal? maxPrice, string? sortOrder, int page = 1, int pageSize = 12)
    {
        var query = _context.Products.AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(x => x.Name.Contains(search));
            ViewData["SearchQuery"] = search;
        }

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

        // Thêm logic sắp xếp
        if (sortOrder == "asc")
        {
            query = query.OrderBy(x => x.Price);
        }
        else if (sortOrder == "desc")
        {
            query = query.OrderByDescending(x => x.Price);
        }
        ViewData["SortOrder"] = sortOrder; // Lưu trạng thái sortOrder

        var totalItems = query.Count();
        var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        var pagination = new PaginationHelper<Product>(items, totalItems, page, pageSize);

        ViewData["Pagination"] = pagination;
        ViewData["PageShop"] = _context.Pages!.AsNoTracking().FirstOrDefault(x => x.Title == "Shop");

        return View();
    }






    public async Task<IActionResult> Details(string name, int id)
{
    var productList = await _context.Products
                         .AsNoTracking()
                         .Include(p => p.Category)
                         .Include(p => p.Reviews).ThenInclude(r => r.User)
                         .ToListAsync();

        var product = productList.FirstOrDefault(p => CreateNameUrl.CreateProductUrl(p.Name) == name && p.Id == id);

        if (product == null)
    {
        return NotFound();
    }

    ViewData["categories"] = await _context.Categories
                                .AsNoTracking()
                                .Include(c => c.Products)
                                .ToListAsync();
        ViewData["relatedproducts"] = productList
        .Where(p => p.Category_id == product.Category_id && p.Id != id)
        .Take(3) // Lấy 3 sản phẩm đầu tiên
        .ToList();

        return View(product);
}

public IActionResult AddToCart(int id, string name, int quantity)
{
    var productList = _context.Products
                              .AsNoTracking()
                              .ToList();

    var product = productList.FirstOrDefault(p => p.Id == id && CreateNameUrl.CreateProductUrl(p.Name) == name);

    if (product == null)
    {
        return NotFound();
    }

    if (quantity > product.Stock)
    {
        TempData["Error"] = "The quantity exceeds available stock.";
        return RedirectToAction("Details", new { id, name });
    }

    var cart = HttpContext.Session.Get<List<CartItem>>("cart") ?? new List<CartItem>();

    var existingItem = cart.FirstOrDefault(item => item.Id == product.Id);

    if (existingItem != null)
    {
        if (existingItem.Quantity + quantity > product.Stock)
        {
            TempData["Error"] = "The quantity in the cart exceeds available stock.";
            return RedirectToAction("Details", new { id, name });
        }

        existingItem.Quantity += quantity;
    }
    else
    {
        cart.Add(new CartItem
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Quantity = quantity,
            Image = product.Image
        });
    }

    HttpContext.Session.Set("cart", cart);

    TempData["Success"] = "Product added to cart successfully.";
    return RedirectToAction("Index", "Cart");
}



    [HttpPost]
    public async Task<IActionResult> AddReview(int productId, string content, int rating)
    {
        var user = HttpContext.Session.Get<User>("userInfo");

        if (user == null)
        {
            TempData["Error"] = "You must be logged in to leave a review.";

            var productList = await _context.Products
                                            .AsNoTracking()
                                            .ToListAsync();

            var product = productList.FirstOrDefault(p => p.Id == productId);

            if (product == null)
            {
                return NotFound();
            }

            var name = CreateNameUrl.CreateProductUrl(product.Name);
            return RedirectToAction("Details", new { id = productId, name });
        }

        var hasPurchased = await _context.Orders
            .Where(o => o.User_id == user.Id && o.Status == "Completed")
            .AnyAsync(o => o.Order_Details.Any(od => od.Product_id == productId));

        if (!hasPurchased)
        {
            TempData["Error"] = "You can only review products you have purchased in a successful order.";

            var product = await _context.Products.FindAsync(productId);
            var name = CreateNameUrl.CreateProductUrl(product?.Name ?? "");

            return RedirectToAction("Details", new { id = productId, name });
        }

        var review = new Review
        {
            Product_id = productId,
            User_id = user.Id,
            Comment = content,
            Rating = rating,
            CreatedDate = DateTime.Now,
            CreatedBy = user.Name
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Your review has been submitted successfully.";

        var redirectProduct = await _context.Products
                                            .AsNoTracking()
                                            .FirstOrDefaultAsync(p => p.Id == productId);

        if (redirectProduct == null)
        {
            return NotFound();
        }

        var redirectName = CreateNameUrl.CreateProductUrl(redirectProduct.Name);
        return RedirectToAction("Details", new { id = productId, name = redirectName });
    }
}

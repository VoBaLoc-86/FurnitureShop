using System.Diagnostics;
using FurnitureShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurnitureShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly FurnitureShopContext _context;
        public HomeController(ILogger<HomeController> logger, FurnitureShopContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            ViewData["PageHome"] = _context.Pages.AsNoTracking()
                                         .Where(x => x.Title == "Home")
                                         .FirstOrDefault();
            ViewData["PageAboutUs"] = _context.Pages.AsNoTracking()
                                         .Where(x => x.Title == "About Us")
                                         .FirstOrDefault();
            ViewData["BannerHome"] = _context.Banners.AsNoTracking()
                                        .Where(x => x.Title == "BannerHome")
                                        .FirstOrDefault();
            ViewData["PageShop"] = _context.Pages.AsNoTracking()
                                         .Where(x => x.Title == "Shop")
                                         .FirstOrDefault();
            ViewData["HotProducts"] = _context.Products.AsNoTracking()
                                        .Take(3)
                                        .ToList();
            ViewData["PageService"] = _context.Pages.AsNoTracking()
                                         .Where(x => x.Title == "Service")
                                         .FirstOrDefault();
            ViewData["HotServices"] = _context.Features.AsNoTracking()
                                        .Take(4)
                                        .ToList();
            ViewData["HotReviews"] = _context.Reviews.AsNoTracking()
                                        .Take(3)
                                        .Include(x=>x.User)
                                        .ToList();
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["PageContact"] = _context.Pages.AsNoTracking()
                             .Where(x => x.Title == "Contact Us")
                             .FirstOrDefault();

            ViewData["BannerContact"] = _context.Banners.AsNoTracking()
                                        .Where(x => x.Title == "BannerContact")
                                        .FirstOrDefault();
            return View();
        }

        public IActionResult Services()
        {
            ViewData["PageService"] = _context.Pages.AsNoTracking()
                             .Where(x => x.Title == "Service")
                             .FirstOrDefault();

            ViewData["BannerService"] = _context.Banners.AsNoTracking()
                                        .Where(x => x.Title == "BannerService")
                                        .FirstOrDefault();
            ViewData["HotProducts"] = _context.Products.AsNoTracking()
                                        .Take(3)
                                        .ToList();

            ViewData["AllServices"] = _context.Features.AsNoTracking()
                                        .ToList();
            ViewData["ServiceReviews"] = _context.Reviews.AsNoTracking()
                                        .Take(3)
                                        .Include(x => x.User)
                                        .ToList();
            return View();
        }
        public IActionResult About()
        {
            ViewData["PageAboutUs"] = _context.Pages.AsNoTracking()
                                         .Where(x => x.Title == "About Us")
                                         .FirstOrDefault();
            
            ViewData["BannerAboutUs"] = _context.Banners.AsNoTracking()
                                        .Where(x => x.Title == "BannerAboutUs")
                                        .FirstOrDefault();

            ViewData["AboutServices"] = _context.Features.AsNoTracking()
                                        .Take(4)
                                        .ToList();
            ViewData["AboutReviews"] = _context.Reviews.AsNoTracking()
                                        .Take(3)
                                        .Include(x => x.User)
                                        .ToList();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

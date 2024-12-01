using Microsoft.AspNetCore.Mvc;

namespace FurnitureShop.Controllers
{
    public class ShopController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}

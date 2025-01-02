using FurnitureShop.Areas.Admin.DTOs.request;
using FurnitureShop.Models;
using FurnitureShop.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurnitureShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LoginController : Controller
    {
        private readonly FurnitureShopContext _context;
        public LoginController(FurnitureShopContext context)
        {
            _context = context;

        }
        public async Task<IActionResult> Index()
        {
            var login = Request.Cookies.Get<LoginDTO>("UserCrential");
            if (login != null)
            {
                var result = await _context.AdminUsers!.AsNoTracking().FirstOrDefaultAsync(x => x.Name == login.Name && x.Password == login.Password);
                if (result != null)
                {
                    HttpContext.Session.Set<AdminUser>("adminInfo", result);
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginDTO login)
        {
            var result = await _context.AdminUsers!.AsNoTracking().FirstOrDefaultAsync(x => x.Name == login.Name && x.Password == login.Password);
            if (result != null)
            {
                if (login.RememberMe)
                {
                    Response.Cookies.Append<LoginDTO>("UserCrential", login, new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddMinutes(10),
                        HttpOnly = true,
                        IsEssential = true
                    });
                }
                HttpContext.Session.Set<AdminUser>("adminInfo", result);
                return RedirectToAction("Index", "Home");
            }
            else
            {
               
                TempData["Message"] = "Wrong username or password";
                return RedirectToAction("Index", "Login");
            }
            
        }
    }
}

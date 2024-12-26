using System.Diagnostics;
using System.Security.Claims;
using FurnitureShop.Areas.Admin.DTOs.request;
using FurnitureShop.Models;
using FurnitureShop.Services;
using FurnitureShop.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Newtonsoft.Json;
using NuGet.Protocol.Plugins;

namespace FurnitureShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly FurnitureShopContext _context;
        private readonly IEmailSender _emailSender;
        public HomeController(IEmailSender emailSender, ILogger<HomeController> logger, FurnitureShopContext context)
        {
            _emailSender = emailSender;
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
                                        .Include(x => x.User)
                                        .ToList();
            ViewData["FacebookLink"] = _context.Settings.AsNoTracking()
                                   .Where(x => x.Name == "FacebookLink")
                                   .FirstOrDefault();
            ViewData["InstagramLink"] = _context.Settings.AsNoTracking()
                                   .Where(x => x.Name == "InstagramLink")
                                   .FirstOrDefault();
            ViewData["TwitterLink"] = _context.Settings.AsNoTracking()
                                   .Where(x => x.Name == "TwitterLink")
                                   .FirstOrDefault();
            ViewData["LinkedInLink"] = _context.Settings.AsNoTracking()
                                   .Where(x => x.Name == "LinkedInLink")
                                   .FirstOrDefault();
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["PageContact"] = _context.Pages.AsNoTracking()
                             .Where(x => x.Title == "Contact Us")
                             .FirstOrDefault();


            ViewData["ContactAddress"] = _context.Settings.AsNoTracking()
                                    .Where(x => x.Name == "ContactAddress")
                                    .FirstOrDefault();
            ViewData["ContactEmail"] = _context.Settings.AsNoTracking()
                                    .Where(x => x.Name == "ContactEmail")
                                    .FirstOrDefault();
            ViewData["ContactPhone"] = _context.Settings.AsNoTracking()
                                    .Where(x => x.Name == "ContactPhone")
                                    .FirstOrDefault();
            return View();
        }

        public IActionResult Services()
        {
            ViewData["PageService"] = _context.Pages.AsNoTracking()
                             .Where(x => x.Title == "Service")
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
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(User login)
        {
            var user = await _context.Users!
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == login.Email);

            if (user != null)
            {
                // Sử dụng BCrypt để xác minh mật khẩu
                var passwordMatch = BCrypt.Net.BCrypt.Verify(login.Password, user.Password);

                if (passwordMatch)
                {
                    HttpContext.Session.Set<User>("userInfo", user);
                    return RedirectToAction("Index", "Home");
                }
            }

            TempData["Message"] = "Wrong username or password";
            return RedirectToAction("Login", "Home");
        }




        public IActionResult Logout()
        {
            // Xóa toàn bộ dữ liệu trong session
            HttpContext.Session.Clear();

            // Xóa cookie lưu trữ session
            if (Request.Cookies.Count > 0)
            {
                foreach (var cookie in Request.Cookies.Keys)
                {
                    Response.Cookies.Delete(cookie);
                }
            }

            // Điều hướng về trang Login
            return RedirectToAction("Index", "Home");
        }

        // Register
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại.");
                    return View(user);
                }

                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                user.EmailConfirmationToken = Guid.NewGuid().ToString();
                user.EmailConfirmed = false;

                var pendingUser = new User
                {
                    Name = user.Name,
                    Email = user.Email,
                    Password = user.Password,
                    CreatedBy = user.Name,
                    EmailConfirmationToken = user.EmailConfirmationToken,
                    EmailConfirmed = user.EmailConfirmed,
                    Address = user.Address,
                    Phone = user.Phone
                };

                HttpContext.Session.SetString("PendingUser", JsonConvert.SerializeObject(pendingUser));

                var confirmationLink = Url.Action("ConfirmEmail", "Home",
                    new { token = user.EmailConfirmationToken, email = user.Email }, Request.Scheme);

                await _emailSender.SendEmailAsync(user.Email, "Xác minh email",
                    $"Vui lòng nhấn vào đường link để xác minh email của bạn: {confirmationLink}");

                return RedirectToAction("ConfirmEmailSent");
            }

            return View(user);
        }



        public IActionResult ConfirmEmailSent()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                return BadRequest("Invalid token or email.");
            }

            var pendingUserJson = HttpContext.Session.GetString("PendingUser");
            if (string.IsNullOrEmpty(pendingUserJson))
            {
                return BadRequest("No pending user found.");
            }

            var user = JsonConvert.DeserializeObject<User>(pendingUserJson);
            if (user.Email != email || user.EmailConfirmationToken != token)
            {
                return BadRequest("Invalid token or email.");
            }

            user.EmailConfirmed = true;
            user.EmailConfirmationToken = null;

            // Lưu người dùng vào cơ sở dữ liệu
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Xóa thông tin người dùng khỏi session
            HttpContext.Session.Remove("PendingUser");

            // Chuyển hướng đến trang đăng nhập
            return RedirectToAction("Login");
        }


    }
}

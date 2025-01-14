using System.Diagnostics;
using System.Security.Claims;
using FurnitureShop.Areas.Admin.DTOs.request;
using FurnitureShop.DTO;
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
using LoginDTO = FurnitureShop.DTO.LoginDTO;
namespace FurnitureShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly FurnitureShopContext _context;
        private readonly IEmailSender _emailSender;
        private readonly IHttpClientFactory _httpClientFactory; 
        private readonly string _recaptchaSecret = "6Ldd7rYqAAAAAPTLP-KjvOZUxoF0wEt-N4OMLZGX";
        public HomeController(IEmailSender emailSender, ILogger<HomeController> logger, FurnitureShopContext context,IHttpClientFactory httpClientFactory)
        {
            _emailSender = emailSender;
            _logger = logger;
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["PageHome"] = _context.Pages.AsNoTracking()
                                         .Where(x => x.Title == "Home")
                                         .FirstOrDefault();
            ViewData["PageAboutUs"] = _context.Pages.AsNoTracking()
                                         .Where(x => x.Title == "About Us")
                                         .FirstOrDefault();

            ViewData["PageShop"] =  _context.Pages.AsNoTracking()
                                         .Where(x => x.Title == "Shop")
                                         .FirstOrDefault();
            ViewData["HotProducts"] =  _context.Products.AsNoTracking()
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
                                        .Take(8).ToList();
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
        public async Task<IActionResult> Login(LoginDTO login)
        {
            // Lấy giá trị reCAPTCHA từ request
            var recaptchaResponse = Request.Form["g-recaptcha-response"];
            var isValidCaptcha = await ValidateRecaptcha(recaptchaResponse);

            // Kiểm tra CAPTCHA
            if (!isValidCaptcha)
            {
                TempData["Message"] = "The CAPTCHA is not valid. Please try again.";
                return View(login);
            }

            // Tìm người dùng dựa trên email
            var user = await _context.Users!
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == login.Email);

            if (user != null)
            {
                // Xác minh mật khẩu bằng BCrypt
                var passwordMatch = BCrypt.Net.BCrypt.Verify(login.Password, user.Password);

                if (passwordMatch)
                {
                    // Lưu cookie nếu RememberMe được chọn
                    if (login.RememberMe)
                    {
                        Response.Cookies.Append(
                            "UserCredential",
                            login.Email,
                            new CookieOptions
                            {
                                Expires = DateTimeOffset.UtcNow.AddMinutes(10),
                                HttpOnly = true,
                                IsEssential = true
                            }
                        );
                    }

                    // Lưu thông tin user vào session
                    HttpContext.Session.Set<User>("userInfo", user);

                    return RedirectToAction("Index", "Home");
                }
            }

            // Sai tài khoản hoặc mật khẩu
            TempData["Message"] = "Wrong email or password";

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
                // Lấy giá trị reCAPTCHA từ request
                var recaptchaResponse = Request.Form["g-recaptcha-response"];
                var isValidCaptcha = await ValidateRecaptcha(recaptchaResponse);

                // Kiểm tra CAPTCHA
                if (!isValidCaptcha)
                {
                    ModelState.AddModelError(string.Empty, "The CAPTCHA is not valid. Please try again.");
                    return View(user);
                }

                // Kiểm tra email có tồn tại trong cơ sở dữ liệu không
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại.");
                    return View(user);
                }

                // Mã hóa mật khẩu
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

                // Tạo token xác minh email
                user.EmailConfirmationToken = Guid.NewGuid().ToString();
                user.EmailConfirmed = false;

                // Lưu thông tin người dùng đang chờ xác nhận
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

                // Lưu thông tin người dùng vào Session
                HttpContext.Session.SetString("PendingUser", JsonConvert.SerializeObject(pendingUser));

                // Tạo link xác nhận email
                var confirmationLink = Url.Action(
                    "ConfirmEmail",
                    "Home",
                    new { token = user.EmailConfirmationToken, email = user.Email },
                    Request.Scheme
                );

                // Gửi email xác minh
                await _emailSender.SendEmailAsync(
                    user.Email,
                    "Xác minh email",
                    $"Vui lòng nhấn vào đường link để xác minh email của bạn: {confirmationLink}"
                );

                return RedirectToAction("ConfirmEmailSent");
            }

            return View(user);
        }

        private async Task<bool> ValidateRecaptcha(string recaptchaResponse)
        {
            var requestUrl = $"https://www.google.com/recaptcha/api/siteverify?secret={_recaptchaSecret}&response={recaptchaResponse}";
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetStringAsync(requestUrl);
            var recaptchaResult = JsonConvert.DeserializeObject<RecaptchaResponse>(response);

            return recaptchaResult.Success;
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
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDTO forgotPassword)
        {
            // Lấy giá trị reCAPTCHA từ request
            var recaptchaResponse = Request.Form["g-recaptcha-response"];
            var isValidCaptcha = await ValidateRecaptcha(recaptchaResponse);

            // Kiểm tra CAPTCHA
            if (!isValidCaptcha)
            {
                TempData["Message"] = "The CAPTCHA is not valid. Please try again.";
                return View(forgotPassword);
            }

            var user = await _context.Users!
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == forgotPassword.Email);

            if (user != null)
            {
                // Tạo mã thông báo đặt lại mật khẩu
                user.ResetToken = Guid.NewGuid().ToString();
                _context.Update(user);
                await _context.SaveChangesAsync();

                // Tạo liên kết đặt lại mật khẩu
                var resetLink = Url.Action("ResetPassword", "Home",
                    new { token = user.ResetToken, email = user.Email }, Request.Scheme);

                // Gửi email đặt lại mật khẩu
                await _emailSender.SendEmailAsync(user.Email, "Đặt lại mật khẩu",
                    $"Vui lòng nhấn vào đường link để đặt lại mật khẩu của bạn: {resetLink}");

                TempData["Message"] = "Reset password link has been sent to your email.";
                return RedirectToAction("Login", "Home");
            }

            TempData["Message"] = "Email address not found.";
            return RedirectToAction("ForgotPassword", "Home");
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                return BadRequest("Invalid token or email.");
            }

            var model = new ResetPasswordDTO { Token = token, Email = email };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPassword)
        {
            if (resetPassword.NewPassword != resetPassword.ConfirmNewPassword)
            {
                TempData["Message"] = "Passwords do not match.";
                return RedirectToAction("ResetPassword", "Home", new { token = resetPassword.Token, email = resetPassword.Email });
            }

            var user = await _context.Users!
                .FirstOrDefaultAsync(x => x.Email == resetPassword.Email && x.ResetToken == resetPassword.Token);

            if (user != null)
            {
                // Đặt lại mật khẩu
                user.Password = BCrypt.Net.BCrypt.HashPassword(resetPassword.NewPassword);
                user.ResetToken = null; // Xóa mã thông báo sau khi sử dụng

                _context.Update(user);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Password has been reset successfully.";
                return RedirectToAction("Login", "Home");
            }

            TempData["Message"] = "Invalid reset token.";
            return RedirectToAction("ResetPassword", "Home", new { token = resetPassword.Token, email = resetPassword.Email });
        }




    }
}

using FurnitureShop.Models;
using FurnitureShop.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FurnitureShop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Cấu hình DbContext cho SQL Server
            builder.Services.AddDbContext<FurnitureShopContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Thêm các dịch vụ của MVC
            builder.Services.AddControllersWithViews();

            // Cấu hình Session
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(1); // Thiết lập thời gian hết hạn session
                options.Cookie.HttpOnly = true;         // Chỉ server mới có thể truy cập session cookie
                options.Cookie.IsEssential = true;      // Đảm bảo tuân thủ GDPR
            });

            // Thêm dịch vụ Identity và cấu hình liên quan đến xác thực
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true; // Yêu cầu xác thực email
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Tokens.EmailConfirmationTokenProvider = "emailconfirmation";
            })
            .AddEntityFrameworkStores<FurnitureShopContext>()  // Sử dụng Entity Framework để lưu trữ thông tin Identity
            .AddDefaultTokenProviders();

            // Cấu hình thời hạn cho token xác thực email
            builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
                opt.TokenLifespan = TimeSpan.FromSeconds(10));

            // Cấu hình dịch vụ gửi email
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddTransient<IEmailSender, EmailSender>();

            // Cấu hình VNPaySettings
            builder.Services.Configure<VNPaySettings>(builder.Configuration.GetSection("VNPay"));

            // Thêm IHttpClientFactory cho reCAPTCHA
            builder.Services.AddHttpClient();

            // Tạo ứng dụng từ builder
            var app = builder.Build();

            // Cấu hình Middleware cho HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();  // HSTS cho môi trường sản xuất
            }
            app.UseHttpsRedirection();  // Chuyển hướng HTTP sang HTTPS
            app.UseStaticFiles();       // Cung cấp các tệp tĩnh

            app.UseRouting();           // Cấu hình routing
            app.UseSession();           // Sử dụng session
            app.UseAuthentication();    // Sử dụng xác thực
            app.UseAuthorization();     // Sử dụng phân quyền

            // Định tuyến cho các controller và areas
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            // Định tuyến tùy chỉnh cho URL thân thiện
            app.MapControllerRoute(
                name: "friendlyRoute",
                pattern: "Shop/Details/{name}-{id}",
                defaults: new { controller = "Shop", action = "Details" });

            // Định tuyến mặc định cho controller
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Chạy ứng dụng
            app.Run();
        }
    }
}

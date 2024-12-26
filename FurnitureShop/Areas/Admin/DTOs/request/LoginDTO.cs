namespace FurnitureShop.Areas.Admin.DTOs.request
{
    public class LoginDTO
    {
        public required string Name { get; set; }
        public required string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}

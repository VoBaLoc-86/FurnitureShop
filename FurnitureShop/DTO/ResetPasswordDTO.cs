namespace FurnitureShop.DTO
{
    public class ResetPasswordDTO
    {
        public required string Token { get; set; }
        public required string Email { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmNewPassword { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace FurnitureShop.Models
{
    public class AdminUser:BaseModel
    {
        [Key]
        public int USE_ID { get; set; }
        public required string Username { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password ít nhất 6 kí tự.")]
        public required string Password { get; set; }
        public string? DisplayName { get; set; }
        [EmailAddress(ErrorMessage = "Không đúng định dạng Email.")]
        public string? Email { get; set; }
        [Phone]
        [RegularExpression(@"^(090|091)[0-9]{7}$", ErrorMessage = "Invalid phone number format. Example: 090xxxxxxx or 091xxxxxxx.")]
        public string? Phone {  get; set; }

    }
}

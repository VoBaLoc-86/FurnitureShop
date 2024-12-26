using System.ComponentModel.DataAnnotations;

namespace FurnitureShop.Models
{
    public class AdminUser:BaseModel
    {
        [Key]
        public int USE_ID { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Display Name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Display Name must be between 3 and 100 characters.")]
        public string? DisplayName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "The email address is not in the correct format.")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Phone is required.")]
        [Phone]
        [RegularExpression(@"^(090|091)[0-9]{7}$", ErrorMessage = "Invalid phone number format. Example: 090xxxxxxx or 091xxxxxxx.")]
        public string? Phone { get; set; }

    }
}

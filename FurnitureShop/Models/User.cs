using System.ComponentModel.DataAnnotations;

namespace FurnitureShop.Models
{
    public class User:BaseModel
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Không đúng định dạng Email.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password ít nhất 6 kí tự.")]
        public required string Password { get; set; }
        
        public string? Address { get; set; }
        
        public string? Phone { get; set; }
        public bool EmailConfirmed { get; set; }  // Thêm cột EmailConfirmed
        public string? EmailConfirmationToken { get; set; }
        public virtual ICollection<Order>? Orders { get; set; }
        public virtual ICollection<Review>? Reviews { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace FurnitureShop.Models
{
    public class User:BaseModel
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
        [EmailAddress(ErrorMessage = "Không đúng định dạng Email.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password ít nhất 6 kí tự.")]
        public required string Password { get; set; }
        public virtual ICollection<Order>? Orders { get; set; }
        public virtual ICollection<Review>? Reviews { get; set; }
    }
}

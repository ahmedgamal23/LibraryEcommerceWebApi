using System.ComponentModel.DataAnnotations;

namespace LibraryEcommerceWeb.Domain.Entities
{
    public class User
    {
        [Key, Required]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = UserRole.Customer.ToString(); // Admin, Vendor, Customer
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool? IsConfirmed { get; set; } = false;
        public bool? IsDeleted { get; set; } = false;

        [Required]
        public string ImagePath { get; set; } = string.Empty;
        public ICollection<Order>? Orders { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<ShoppingCartItem>? ShoppingCartItems { get; set; }
    }
}

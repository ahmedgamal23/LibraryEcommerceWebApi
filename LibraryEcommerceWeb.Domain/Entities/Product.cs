using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryEcommerceWeb.Domain.Entities
{
    public class Product
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool IsDigital { get; set; }
        public string? DigitalUrl { get; set; } // Only for digital products
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string ImagePath { get; set; } = string.Empty;

        // Foreign key
        [ForeignKey(nameof(Category))]
        public string CategoryId { get; set; } = string.Empty;
        public Category? Category { get; set; }

        // Foreign key
        [ForeignKey(nameof(Vendor))]
        public string VendorId { get; set; } = string.Empty;
        public User? Vendor { get; set; }
        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}

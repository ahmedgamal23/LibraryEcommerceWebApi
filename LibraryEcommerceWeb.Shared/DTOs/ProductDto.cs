using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace LibraryEcommerceWeb.Shared.DTOs
{
    public class ProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool IsDigital { get; set; }
        public string? DigitalUrl { get; set; } // Only for digital products
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public IFormFile? FormFile { get; set; }
        public string VendorId { get; set; } = string.Empty;
        public string CategoryId { get; set; } = string.Empty;

        [JsonIgnore]
        public ICollection<OrderItemDto>? OrderItemDtos { get; set; }
    }
}


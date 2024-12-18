using System.Text.Json.Serialization;

namespace LibraryEcommerceWeb.Shared.DTOs
{
    public class OrderDto
    {
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty; // Pending, Processing, Completed, Canceled
        public string UserId { get; set; } = string.Empty;

        [JsonIgnore]
        public ICollection<OrderItemDto>? OrderItemDtos { get; set; }
    }
}

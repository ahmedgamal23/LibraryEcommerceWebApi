using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LibraryEcommerceWeb.Domain.Entities
{
    public class Order
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty; // Pending, Processing, Completed, Canceled

        [ForeignKey(nameof(Customer))]
        public string UserId { get; set; } = string.Empty;
        [JsonIgnore]
        public User? Customer { get; set; }
        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}

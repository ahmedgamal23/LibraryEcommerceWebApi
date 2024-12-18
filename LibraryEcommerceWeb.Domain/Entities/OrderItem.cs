using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LibraryEcommerceWeb.Domain.Entities
{
    public class OrderItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [ForeignKey(nameof(Product))]
        public string ProductId { get; set; } = string.Empty;
        [JsonIgnore]
        public Product? Product { get; set; }

        [ForeignKey(nameof(Order))]
        public string OrderId { get; set; } = string.Empty;
        [JsonIgnore]
        public Order? Order { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}

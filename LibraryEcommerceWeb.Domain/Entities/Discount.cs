using System.Text.Json.Serialization;

namespace LibraryEcommerceWeb.Domain.Entities
{
    public class Discount
    {
        [JsonIgnore]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Code { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }
        public DateTime ExpirationDate { get; set; }

        [JsonIgnore]
        public ICollection<Order>? Orders { get; set; }
    }
}



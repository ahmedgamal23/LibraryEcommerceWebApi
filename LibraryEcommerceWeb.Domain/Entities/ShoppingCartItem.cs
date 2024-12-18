using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LibraryEcommerceWeb.Domain.Entities
{
    public class ShoppingCartItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [ForeignKey(nameof(Product))]
        public string ProductId { get; set; } = string.Empty;
        [JsonIgnore]
        public Product? Product { get; set; }

        [ForeignKey(nameof(Customer))]
        public string UserId { get; set; } = string.Empty;
        [JsonIgnore]
        public User? Customer { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "The field Quantity must be greater than 0.")]
        public int Quantity { get; set; }
    }

}

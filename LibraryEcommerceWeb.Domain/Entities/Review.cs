using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LibraryEcommerceWeb.Domain.Entities
{
    public class Review
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Range(1, 5)]
        public int Rating { get; set; } // Rating out of 5
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey(nameof(Product))]
        public string ProductId { get; set; } = string.Empty;
        [JsonIgnore]
        public Product? Product { get; set; }

        [ForeignKey(nameof(Customer))]
        public string UserId { get; set; } = string.Empty;
        [JsonIgnore]
        public User? Customer { get; set; }
    }

}

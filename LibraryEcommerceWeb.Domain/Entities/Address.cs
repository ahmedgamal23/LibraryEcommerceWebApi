using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryEcommerceWeb.Domain.Entities
{
    public class Address
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = string.Empty;
        public User? User { get; set; }
    }
}


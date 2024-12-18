using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryEcommerceWeb.Domain.Entities
{
    public class Payment
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [ForeignKey(nameof(Order))]
        public string OrderId { get; set; } = string.Empty;
        public Order? Order { get; set; }
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; } = string.Empty; // Paid, Failed, Pending
        public string PaymentMethod { get; set; } = string.Empty; // Stripe, PayPal, etc.
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    }

}

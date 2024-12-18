namespace LibraryEcommerceWeb.Shared.DTOs
{
    public class PaymentDto
    {
        public string OrderId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; } = string.Empty; // Paid, Failed, Pending
        public string PaymentMethod { get; set; } = string.Empty; // Stripe, PayPal, etc.
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

    }
}

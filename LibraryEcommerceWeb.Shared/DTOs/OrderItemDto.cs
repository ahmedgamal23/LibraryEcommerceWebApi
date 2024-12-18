namespace LibraryEcommerceWeb.Shared.DTOs
{
    public class OrderItemDto
    {
        public string ProductId { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}

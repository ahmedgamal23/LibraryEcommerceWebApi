namespace LibraryEcommerceWeb.Shared.DTOs
{
    public class ShoppingCartItemDto
    {
        public string ProductId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}


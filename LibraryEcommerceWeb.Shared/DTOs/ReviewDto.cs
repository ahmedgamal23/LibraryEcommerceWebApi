using System.ComponentModel.DataAnnotations;

namespace LibraryEcommerceWeb.Shared.DTOs
{
    public class ReviewDto
    {
        [Range(1, 5)]
        public int Rating { get; set; } // Rating out of 5
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string ProductId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }
}

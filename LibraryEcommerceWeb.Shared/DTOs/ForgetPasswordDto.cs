using System.ComponentModel.DataAnnotations;

namespace LibraryEcommerceWeb.Shared.DTOs
{
    public class ForgetPasswordDto
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string NewPassword { get; set; } = string.Empty;
    }
}

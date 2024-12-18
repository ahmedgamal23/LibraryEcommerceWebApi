using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LibraryEcommerceWeb.Shared.DTOs
{
    public class CategoryDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public IFormFile? FormFile { get; set; }

    }
}

using System.ComponentModel.DataAnnotations;

namespace AmazonClone.Application.DTOs
{
    public class GoogleLoginRequestDto
    {
        [Required]
        public string IdToken { get; set; } = string.Empty;
    }
}

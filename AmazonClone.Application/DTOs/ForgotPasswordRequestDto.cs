using System.ComponentModel.DataAnnotations;

namespace AmazonClone.Application.DTOs
{
    public class ForgotPasswordRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}

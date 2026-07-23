using System;
using System.ComponentModel.DataAnnotations;

namespace AmazonClone.Application.DTOs
{
    public class VerifyEmailRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Token { get; set; } = string.Empty;
    }
}

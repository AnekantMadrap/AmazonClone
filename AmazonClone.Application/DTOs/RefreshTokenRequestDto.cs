using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AmazonClone.Application.DTOs
{
    public class RefreshTokenRequestDto
    {
        [Required]
        public string AccessToken { get; set; } = string.Empty;

        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}

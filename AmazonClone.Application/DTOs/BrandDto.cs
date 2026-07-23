using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AmazonClone.Application.DTOs
{
    public class BrandDto
    {
        public int BrandId { get; set; }

        [Required(ErrorMessage = "Brand name is required.")]
        [StringLength(100, ErrorMessage = "Brand name cannot exceed 100 characters.")]
        public string BrandName { get; set; } = string.Empty;

        public string? LogoUrl { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        public string Status { get; set; } = "Active";

        public int DisplayOrder { get; set; } = 0;
    }
}

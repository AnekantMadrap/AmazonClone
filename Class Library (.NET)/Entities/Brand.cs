using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Class_Library__.NET_.Entities
{
    public class Brand
    {
        [Key]
        public int BrandId { get; set; }

        [Required]
        [StringLength(100)]
        public string BrandName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? LogoUrl { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Active";

        public int DisplayOrder { get; set; } = 0;
    }
}

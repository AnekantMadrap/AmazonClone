using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AmazonClone.Application.DTOs
{
    public class ProductVariantDto
    {
        public int VariantId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [StringLength(50)]
        public string? Color { get; set; }

        [StringLength(50)]
        public string? Size { get; set; }

        [StringLength(50)]
        public string? RAM { get; set; }

        [StringLength(50)]
        public string? Storage { get; set; }

        [Required]
        [StringLength(100)]
        public string SKU { get; set; } = string.Empty;

        [Range(0.01, 99999999.99)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; } = 0;

        public bool IsDefault { get; set; } = false;
    }
}

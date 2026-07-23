using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Class_Library__.NET_.Entities
{
    public class ProductVariant
    {
        [Key]
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

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [NotMapped]
        public int StockQuantity { get; set; } = 0;

        [NotMapped]
        public bool IsDefault { get; set; } = false;

        // Navigation Property
        [ForeignKey("ProductId")]
        public virtual Products? Product { get; set; }
    }
}

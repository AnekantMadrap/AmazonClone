using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Class_Library__.NET_.Entities
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }

        [Required]
        public int CartId { get; set; }

        [Required]
        public int ProductId { get; set; }

        public int? VariantId { get; set; }

        [Required]
        public int Quantity { get; set; } = 1;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        // Navigation properties
        [ForeignKey("CartId")]
        public virtual Cart? Cart { get; set; }

        [ForeignKey("ProductId")]
        public virtual Products? Product { get; set; }

        [ForeignKey("VariantId")]
        public virtual ProductVariant? Variant { get; set; }
    }
}

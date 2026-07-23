using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Class_Library__.NET_.Entities
{
    public class Inventory
    {
        [Key]
        public int InventoryId { get; set; }

        [Required]
        public int ProductId { get; set; }

        public int? VariantId { get; set; }

        public int Quantity { get; set; } = 0;

        public int ReservedQuantity { get; set; } = 0;

        public int ReorderLevel { get; set; } = 10;

        public DateTime LastRestockedDate { get; set; } = DateTime.UtcNow;

        [ForeignKey("ProductId")]
        public virtual Products? Product { get; set; }

        [ForeignKey("VariantId")]
        public virtual ProductVariant? Variant { get; set; }
    }
}

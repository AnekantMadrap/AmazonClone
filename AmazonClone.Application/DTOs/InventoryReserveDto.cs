using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AmazonClone.Application.DTOs
{
    public class InventoryReserveDto
    {
        [Required]
        public int ProductId { get; set; }

        public int? VariantId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
    }
}

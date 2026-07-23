using System;
using System.Collections.Generic;
using System.Text;

namespace AmazonClone.Application.DTOs
{
    public class InventoryCheckDto
    {
        public int ProductId { get; set; }
        public int? VariantId { get; set; }
        public bool IsAvailable { get; set; }
        public int QuantityAvailable { get; set; }
        public bool IsLowStock { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}

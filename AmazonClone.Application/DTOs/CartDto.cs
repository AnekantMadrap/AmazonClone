using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AmazonClone.Application.DTOs
{
    public class CartItemDto
    {
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? PrimaryImageUrl { get; set; }
        public int? VariantId { get; set; }
        public string? VariantInfo { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => UnitPrice * Quantity;
        public int AvailableStock { get; set; }
    }

    public class CartDto
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
        public int TotalItems => Items.Sum(i => i.Quantity);
        public decimal Subtotal => Items.Sum(i => i.TotalPrice);
    }

    public class AddCartItemDto
    {
        [Required]
        public int ProductId { get; set; }

        public int? VariantId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; } = 1;
    }

    public class UpdateCartItemDto
    {
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
        public int Quantity { get; set; }
    }

    public class GuestCartItemDto
    {
        [Required]
        public int ProductId { get; set; }

        public int? VariantId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; } = 1;
    }
}

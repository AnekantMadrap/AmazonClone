using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AmazonClone.Application.DTOs
{
    public class WishlistItemDto
    {
        public int WishlistItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public string? PrimaryImageUrl { get; set; }
        public int AvailableStock { get; set; }
        public DateTime AddedDate { get; set; }
    }

    public class WishlistDto
    {
        public int WishlistId { get; set; }
        public int UserId { get; set; }
        public List<WishlistItemDto> Items { get; set; } = new List<WishlistItemDto>();
    }

    public class AddWishlistItemDto
    {
        [Required]
        public int ProductId { get; set; }
    }
}

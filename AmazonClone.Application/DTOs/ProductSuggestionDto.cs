using System;

namespace AmazonClone.Application.DTOs
{
    public class ProductSuggestionDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public decimal Price { get; set; }
        public string? PrimaryImageUrl { get; set; }
    }
}

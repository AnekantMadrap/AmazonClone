using System;

namespace AmazonClone.Application.DTOs
{
    public class ProductSearchItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int BrandId { get; set; }
        public string? BrandName { get; set; }
        public decimal AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public int AvailableStock { get; set; }
        public string? PrimaryImageUrl { get; set; }
        public string Status { get; set; } = "Active";
        public DateTime CreatedDate { get; set; }
        public int TotalCount { get; set; }
    }
}

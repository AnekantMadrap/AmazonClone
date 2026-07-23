using System;
using System.Collections.Generic;

namespace AmazonClone.Application.DTOs
{
    public class ProductDetailsPageDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public decimal? Weight { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int BrandId { get; set; }
        public string? BrandName { get; set; }
        public decimal AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public int AvailableStock { get; set; }
        public string Status { get; set; } = "Active";
        public DateTime CreatedDate { get; set; }

        public List<ProductVariantDto> Variants { get; set; } = new List<ProductVariantDto>();
        public List<FileUploadResponseDto> Images { get; set; } = new List<FileUploadResponseDto>();
        public List<ProductSearchItemDto> SimilarProducts { get; set; } = new List<ProductSearchItemDto>();
        public List<ProductSearchItemDto> FrequentlyBoughtTogether { get; set; } = new List<ProductSearchItemDto>();
    }
}

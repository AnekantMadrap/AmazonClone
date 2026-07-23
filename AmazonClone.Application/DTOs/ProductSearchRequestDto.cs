using System;

namespace AmazonClone.Application.DTOs
{
    public class ProductSearchRequestDto
    {
        public string? SearchTerm { get; set; }
        public string? Query { get => SearchTerm; set => SearchTerm = value; }
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public decimal? MinRating { get; set; }
        public string? SortBy { get; set; } = "Newest"; // e.g. "PriceAsc", "PriceDesc", "Newest", "Popularity", "Rating"
        public int PageNumber { get; set; } = 1;
        public int PageNo { get => PageNumber; set => PageNumber = value; }
        public int PageSize { get; set; } = 20;
    }
}

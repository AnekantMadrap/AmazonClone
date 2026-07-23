using System;
using System.ComponentModel.DataAnnotations;

namespace AmazonClone.Application.DTOs
{
    public class ProductDto
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Category ID is required.")]
        public int CategoryId { get; set; }

        public string? CategoryName { get; set; }

        [Required(ErrorMessage = "Brand ID is required.")]
        public int BrandId { get; set; }

        public string? BrandName { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(250, ErrorMessage = "Product name cannot exceed 250 characters.")]
        public string ProductName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? ShortDescription { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "SKU is required.")]
        [StringLength(100)]
        public string SKU { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 99999999.99, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        public decimal? DiscountPrice { get; set; }

        public decimal? Weight { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Active";

        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative.")]
        public int Stock { get; set; } = 0;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        public string? PrimaryImageUrl { get; set; }
    }
}

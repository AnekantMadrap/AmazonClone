using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Class_Library__.NET_.Entities
{
    public class Products
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int BrandId { get; set; }

        [Required]
        [StringLength(250)]
        public string ProductName { get; set; }

        [StringLength(500)]
        public string? ShortDescription { get; set; }

        public string? Description { get; set; }

        [Required]
        [StringLength(100)]
        public string SKU { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? DiscountPrice { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Weight { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Active";

        [NotMapped]
        public int Stock { get; set; } = 0;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        // Navigation Properties
        [ForeignKey("CategoryId")]
        public virtual Categories? Category { get; set; }

        [ForeignKey("BrandId")]
        public virtual Brand? Brand { get; set; }

        public virtual ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    }
}

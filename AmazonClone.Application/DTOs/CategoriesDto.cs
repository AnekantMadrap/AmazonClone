using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AmazonClone.Application.DTOs
{
    public class CategoriesDto
    {
        public int CategoryId { get; set; }
        public int? ParentCategoryId { get; set; }
        [Required, MaxLength(100)]
        public string CategoryName { get; set; }
        public string? Image { get; set; }
        [Required]
        [StringLength(100)]
        public int DisplayOrder { get; set; } = 0;
        [Required]
        public string Status { get; set; } = "Active";

    }
}

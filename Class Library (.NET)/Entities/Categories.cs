using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Class_Library__.NET_.Entities
{
    public class Categories
    {
        [Key]
        public int CategoryId { get; set; }
        public int? ParentCategoryId { get; set; }
        [Required]
        [StringLength(150)]
        public string CategoryName { get; set; }=string.Empty;
        [StringLength(500)]
        public string? Image { get; set; }
        public int DisplayOrder { get; set; } = 0;
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Active";
    }
}

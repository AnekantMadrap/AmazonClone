using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Class_Library__.NET_.Entities
{
    public class UploadedFileRecord
    {
        [Key]
        public int FileId { get; set; }

        public int? ProductId { get; set; }

        public int? VariantId { get; set; }

        [Required]
        [StringLength(500)]
        public string FileUrl { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string FileType { get; set; } = "Image"; // "Image" or "Video"

        public bool IsPrimary { get; set; } = false;

        public int SortOrder { get; set; } = 0;

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}

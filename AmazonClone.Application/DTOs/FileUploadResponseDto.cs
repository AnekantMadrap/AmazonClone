using System;
using System.Collections.Generic;
using System.Text;

namespace AmazonClone.Application.DTOs
{
    public class FileUploadResponseDto
    {
        public int FileId { get; set; }
        public int? ProductId { get; set; }
        public int? VariantId { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public string FileType { get; set; } = "Image";
        public bool IsPrimary { get; set; }
        public int SortOrder { get; set; }
    }
}

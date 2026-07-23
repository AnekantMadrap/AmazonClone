using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AmazonClone.Application.DTOs
{
    public class AddressDto
    {
        public int AddressId { get; set; }
        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;
        [Required, Phone]
        public string Mobile { get; set; } = string.Empty;
        [Required, MaxLength(100)]
        public string AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; } = string.Empty;
        [Required, MaxLength(100)]
        public string City { get; set; } = string.Empty;
        [Required, MaxLength(100)]
        public string State { get; set; } = string.Empty;
        [Required, MaxLength(100)]
        public string Country { get; set; } = string.Empty;
        [Required]
        public string ZipCode { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
    }

}

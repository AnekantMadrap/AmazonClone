using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Class_Library__.NET_.Entities
{
    public class Addresses
    {
        [Key]
        public int AddressId { get; set; }
        public int UserId { get; set; }
        [Required]
        public string FullName { get; set; } = string.Empty;
        [Phone]
        public string Mobile { get; set; } = string.Empty;
        [Required]
        public string Country { get; set; } = "India";
        [Required]
        public string State { get; set; } = string.Empty;
        [Required]
        public string City { get; set; } = string.Empty;
        [Required]
        public string ZipCode { get; set; } = string.Empty;
        [Required]
        public string AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; }
        public bool IsDefault { get; set; } = false;
    }
}

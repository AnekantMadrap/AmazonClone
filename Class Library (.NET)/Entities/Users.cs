using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Class_Library__.NET_.Entities
{
    public class Users: IdentityUser<int>
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedDate { get; set; }
        // Navigation property
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
    public class ApplicationRole : IdentityRole<int>
    {
        public ApplicationRole() : base() { }
        public ApplicationRole(string roleName) : base(roleName) { }
    }
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        [Required]
        [MaxLength(500)]
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresUtc { get; set; }
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
        [MaxLength(45)]
        public string? CreatedByIp { get; set; }
        [MaxLength(500)]
        public string? UserAgent { get; set; }
        public DateTime? RevokedUtc { get; set; }
        [MaxLength(45)]
        public string? RevokedByIp { get; set; }
        [MaxLength(500)]
        public string? ReplacedByToken { get; set; }
        [MaxLength(250)]
        public string? ReasonRevoked { get; set; }
        // Computed Properties
        public bool IsExpired => DateTime.UtcNow >= ExpiresUtc;
        public bool IsRevoked => RevokedUtc != null;
        public bool IsActive => !IsRevoked && !IsExpired;
        // Navigation property
        public virtual Users? User { get; set; }
    }
}

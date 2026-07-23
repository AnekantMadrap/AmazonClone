using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Class_Library__.NET_.Entities
{
    [Table("AuditLogs")]
    public class AuditLog
    {
        [Key]
        public int AuditId { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string TableName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string Action { get; set; } = string.Empty; // INSERT, UPDATE, DELETE, LOGIN, REGISTER
        
        [MaxLength(100)]
        public string? RecordId { get; set; }
        
        public string? OldValue { get; set; }
        
        public string? NewValue { get; set; }
        
        [MaxLength(100)]
        public string? CreatedBy { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Class_Library__.NET_.Entities
{
    public class Wishlist
    {
        [Key]
        public int WishlistId { get; set; }

        [Required]
        public int UserId { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual Users? User { get; set; }

        public virtual ICollection<WishlistItem> Items { get; set; } = new List<WishlistItem>();
    }
}

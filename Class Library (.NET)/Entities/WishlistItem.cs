using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Class_Library__.NET_.Entities
{
    public class WishlistItem
    {
        [Key]
        public int WishlistItemId { get; set; }

        [Required]
        public int WishlistId { get; set; }

        [Required]
        public int ProductId { get; set; }

        public DateTime AddedDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("WishlistId")]
        public virtual Wishlist? Wishlist { get; set; }

        [ForeignKey("ProductId")]
        public virtual Products? Product { get; set; }
    }
}

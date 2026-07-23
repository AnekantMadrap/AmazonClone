using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AmazonClone.Application.DTOs
{
    public class ProfileDto
    {
        public string UserId { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        public IList<string> Roles { get; set; } = new List<string>();
    }
}

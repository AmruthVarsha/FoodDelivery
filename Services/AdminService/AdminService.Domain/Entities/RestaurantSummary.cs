using System.ComponentModel.DataAnnotations;

namespace AdminService.Domain.Entities
{
    public class RestaurantSummary
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid RestaurantId { get; set; }

        [Required]
        [StringLength(450)]
        public string OwnerId { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Range(0.0, 5.0)]
        public double Rating { get; set; }

        public int TotalRatings { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public bool IsApproved { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime LastUpdatedAt { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace CatalogService.Domain.Entities
{
    public class ServiceArea
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid RestaurantId { get; set; }

        [Required]
        [RegularExpression(@"^\d{6}$")]
        public string Pincode { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Restaurant Restaurant { get; set; } = null!;
    }
}
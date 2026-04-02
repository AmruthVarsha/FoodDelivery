using System;
using System.ComponentModel.DataAnnotations;

namespace CatalogService.Domain.Entities
{
    public class MenuItem
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid RestaurantId { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(300)]
        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        public bool IsVeg { get; set; }

        public bool IsAvailable { get; set; } = true;

        public int PrepTimeMinutes { get; set; }

        public Restaurant Restaurant { get; set; } = null!;
        public Category Category { get; set; } = null!;
    }
}
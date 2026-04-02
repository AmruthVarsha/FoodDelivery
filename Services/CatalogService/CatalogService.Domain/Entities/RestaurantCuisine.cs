using System;
using System.ComponentModel.DataAnnotations;

namespace CatalogService.Domain.Entities
{
    public class RestaurantCuisine
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid RestaurantId { get; set; }

        [Required]
        public Guid CuisineId { get; set; }

        public Restaurant Restaurant { get; set; } = null!;
        public Cuisine Cuisine { get; set; } = null!;
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CatalogService.Domain.Entities
{
    public class Cuisine
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public ICollection<RestaurantCuisine> RestaurantCuisines { get; set; } = new List<RestaurantCuisine>();
    }
}
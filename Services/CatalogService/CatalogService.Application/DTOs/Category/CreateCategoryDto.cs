using System.ComponentModel.DataAnnotations;

namespace CatalogService.Application.DTOs.Category
{
    /// <summary>Request body for POST /categories (Partner only).</summary>
    public class CreateCategoryDto
    {
        [Required]
        public Guid RestaurantId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        public int DisplayOrder { get; set; } = 0;
    }
}

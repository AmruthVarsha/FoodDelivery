using System.ComponentModel.DataAnnotations;

namespace CatalogService.Application.DTOs.Category
{
    /// <summary>Request body for PUT /categories/{id} (Partner only).</summary>
    public class UpdateCategoryDto
    {
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }
    }
}

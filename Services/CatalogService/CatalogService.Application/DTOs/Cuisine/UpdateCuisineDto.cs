using System.ComponentModel.DataAnnotations;

namespace CatalogService.Application.DTOs.Cuisine
{
    /// <summary>Request body for PUT /cuisines/{id} (Admin only).</summary>
    public class UpdateCuisineDto
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}

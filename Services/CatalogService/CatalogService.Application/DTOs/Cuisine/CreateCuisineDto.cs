using System.ComponentModel.DataAnnotations;

namespace CatalogService.Application.DTOs.Cuisine
{
    /// <summary>Request body for POST /cuisines (Admin only).</summary>
    public class CreateCuisineDto
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;
    }
}

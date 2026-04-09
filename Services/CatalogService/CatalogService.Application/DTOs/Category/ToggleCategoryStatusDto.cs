using System.ComponentModel.DataAnnotations;

namespace CatalogService.Application.DTOs.Category
{
    /// <summary>Request body for PATCH /categories/{id}/toggle-status (Partner only).</summary>
    public class ToggleCategoryStatusDto
    {
        [Required]
        public bool IsActive { get; set; }
    }
}

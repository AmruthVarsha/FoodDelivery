using System.ComponentModel.DataAnnotations;

namespace CatalogService.Application.DTOs.ServiceArea
{
    /// <summary>Request body for POST /serviceareas (Partner only).</summary>
    public class AddServiceAreaDto
    {
        [Required]
        public Guid RestaurantId { get; set; }

        [Required]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Pincode must be exactly 6 digits.")]
        public string Pincode { get; set; } = string.Empty;
    }
}

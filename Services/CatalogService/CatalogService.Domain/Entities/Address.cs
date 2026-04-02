using System.ComponentModel.DataAnnotations;

namespace CatalogService.Domain.Entities
{
    public class Address
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid RestaurantId { get; set; }

        [Required]
        [StringLength(100)]
        public string Street { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string State { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{6}$")]
        public string Pincode { get; set; } = string.Empty;

        public Restaurant Restaurant { get; set; } = null!;
    }
}
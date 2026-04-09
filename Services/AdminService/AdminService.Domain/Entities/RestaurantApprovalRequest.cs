using System.ComponentModel.DataAnnotations;

namespace AdminService.Domain.Entities
{
    public class RestaurantApprovalRequest
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid RestaurantId { get; set; }

        [Required]
        [StringLength(450)]
        public string OwnerId { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string RestaurantName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public DateTime RequestedAt { get; set; }

        [Required]
        public bool IsApproved { get; set; } = false;

        public DateTime? ApprovedAt { get; set; }

        [StringLength(450)]
        public string? ApprovedByAdminId { get; set; }

        [StringLength(500)]
        public string? RejectionReason { get; set; }
    }
}

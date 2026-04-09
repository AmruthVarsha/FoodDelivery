using System.ComponentModel.DataAnnotations;

namespace AdminService.Domain.Entities
{
    public class UserRoleApprovalRequest
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Role { get; set; } = string.Empty;

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

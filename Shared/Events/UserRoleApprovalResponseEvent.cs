namespace Shared.Events
{
    public class UserRoleApprovalResponseEvent
    {
        public string Email { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public string? ApprovedByAdminId { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime ProcessedAt { get; set; }
    }
}

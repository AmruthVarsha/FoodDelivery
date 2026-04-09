namespace Shared.Events
{
    public class UserRoleApprovalRequestEvent
    {
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime RequestedAt { get; set; }
    }
}

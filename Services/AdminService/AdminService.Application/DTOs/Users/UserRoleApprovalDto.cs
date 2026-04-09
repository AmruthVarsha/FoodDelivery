namespace AdminService.Application.DTOs.Users
{
    public class UserRoleApprovalDto
    {
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime RequestedAt { get; set; }
    }
}

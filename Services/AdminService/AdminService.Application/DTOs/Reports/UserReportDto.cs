namespace AdminService.Application.DTOs.Reports
{
    public class UserReportDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        public Dictionary<string, int> UsersByRole { get; set; } = new();
        public int EmailConfirmedUsers { get; set; }
        public int TwoFactorEnabledUsers { get; set; }
    }
}

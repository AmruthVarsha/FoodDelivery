namespace AdminService.Application.DTOs.Reports
{
    public class RestaurantReportDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int TotalRestaurants { get; set; }
        public int ActiveRestaurants { get; set; }
        public int ApprovedRestaurants { get; set; }
        public int PendingApprovalRestaurants { get; set; }
        public double AverageRating { get; set; }
        public Dictionary<string, int> RestaurantsByOwner { get; set; } = new();
    }
}

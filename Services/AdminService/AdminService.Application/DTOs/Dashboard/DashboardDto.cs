namespace AdminService.Application.DTOs.Dashboard
{
    public class DashboardDto
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int ActiveOrders { get; set; }
        public int CancelledOrders { get; set; }
        public int DeliveredOrders { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int TotalRestaurants { get; set; }
        public int ActiveRestaurants { get; set; }
        public int PendingUserApprovals { get; set; }
        public int PendingRestaurantApprovals { get; set; }
    }
}

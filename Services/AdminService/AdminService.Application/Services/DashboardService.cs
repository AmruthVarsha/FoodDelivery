using AdminService.Application.DTOs.Dashboard;
using AdminService.Application.Interfaces.Services;
using AdminService.Domain.Interfaces;

namespace AdminService.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IOrderSummaryRepository _orderSummaryRepository;
        private readonly IUserSummaryRepository _userSummaryRepository;
        private readonly IRestaurantSummaryRepository _restaurantSummaryRepository;
        private readonly IUserRoleApprovalRepository _userRoleApprovalRepository;
        private readonly IRestaurantApprovalRepository _restaurantApprovalRepository;

        public DashboardService(
            IOrderSummaryRepository orderSummaryRepository,
            IUserSummaryRepository userSummaryRepository,
            IRestaurantSummaryRepository restaurantSummaryRepository,
            IUserRoleApprovalRepository userRoleApprovalRepository,
            IRestaurantApprovalRepository restaurantApprovalRepository)
        {
            _orderSummaryRepository = orderSummaryRepository;
            _userSummaryRepository = userSummaryRepository;
            _restaurantSummaryRepository = restaurantSummaryRepository;
            _userRoleApprovalRepository = userRoleApprovalRepository;
            _restaurantApprovalRepository = restaurantApprovalRepository;
        }

        public async Task<DashboardDto> GetDashboardAsync()
        {
            var allOrders = await _orderSummaryRepository.GetAllAsync();
            var totalUsers = await _userSummaryRepository.GetTotalUsersCountAsync();
            var activeUsers = await _userSummaryRepository.GetActiveUsersCountAsync();
            var totalRestaurants = await _restaurantSummaryRepository.GetTotalRestaurantsCountAsync();
            var activeRestaurants = await _restaurantSummaryRepository.GetActiveRestaurantsCountAsync();
            var pendingUserApprovals = (await _userRoleApprovalRepository.GetPendingRequestsAsync()).Count;
            var pendingRestaurantApprovals = (await _restaurantApprovalRepository.GetPendingRequestsAsync()).Count;

            var dashboard = new DashboardDto
            {
                TotalOrders = allOrders.Count,
                TotalRevenue = allOrders.Sum(o => o.TotalAmount),
                ActiveOrders = allOrders.Count(o => o.Status != Domain.Enums.OrderStatus.Delivered && o.Status != Domain.Enums.OrderStatus.CancelledByCustomer),
                CancelledOrders = allOrders.Count(o => o.Status == Domain.Enums.OrderStatus.CancelledByCustomer),
                DeliveredOrders = allOrders.Count(o => o.Status == Domain.Enums.OrderStatus.Delivered),
                TotalUsers = totalUsers,
                ActiveUsers = activeUsers,
                TotalRestaurants = totalRestaurants,
                ActiveRestaurants = activeRestaurants,
                PendingUserApprovals = pendingUserApprovals,
                PendingRestaurantApprovals = pendingRestaurantApprovals
            };

            return dashboard;
        }
    }
}

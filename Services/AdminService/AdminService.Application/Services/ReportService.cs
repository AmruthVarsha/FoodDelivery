using AdminService.Application.DTOs.Reports;
using AdminService.Application.Interfaces.Services;
using AdminService.Domain.Interfaces;

namespace AdminService.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IOrderSummaryRepository _orderSummaryRepository;
        private readonly IUserSummaryRepository _userSummaryRepository;
        private readonly IRestaurantSummaryRepository _restaurantSummaryRepository;

        public ReportService(
            IOrderSummaryRepository orderSummaryRepository,
            IUserSummaryRepository userSummaryRepository,
            IRestaurantSummaryRepository restaurantSummaryRepository)
        {
            _orderSummaryRepository = orderSummaryRepository;
            _userSummaryRepository = userSummaryRepository;
            _restaurantSummaryRepository = restaurantSummaryRepository;
        }

        public async Task<SalesReportDto> GetSalesReportAsync(DateTime from, DateTime to)
        {
            var orders = await _orderSummaryRepository.GetByDateRangeAsync(from, to);

            var dailyBreakdown = orders
                .GroupBy(o => o.PlacedAt.Date)
                .Select(g => new DailySalesDto
                {
                    Date = g.Key,
                    Orders = g.Count(),
                    Revenue = g.Sum(o => o.TotalAmount)
                })
                .OrderBy(d => d.Date)
                .ToList();

            var report = new SalesReportDto
            {
                FromDate = from,
                ToDate = to,
                TotalOrders = orders.Count,
                TotalRevenue = orders.Sum(o => o.TotalAmount),
                DeliveredOrders = orders.Count(o => o.Status == Domain.Enums.OrderStatus.Delivered),
                CancelledOrders = orders.Count(o => o.Status == Domain.Enums.OrderStatus.CancelledByCustomer),
                DailyBreakdown = dailyBreakdown
            };

            return report;
        }

        public async Task<UserReportDto> GetUserReportAsync(DateTime from, DateTime to)
        {
            var allUsers = await _userSummaryRepository.GetAllAsync();
            var usersInRange = allUsers.Where(u => u.CreatedAt >= from && u.CreatedAt <= to).ToList();

            var usersByRole = usersInRange
                .GroupBy(u => u.Role)
                .ToDictionary(g => g.Key, g => g.Count());

            var report = new UserReportDto
            {
                FromDate = from,
                ToDate = to,
                TotalUsers = usersInRange.Count,
                ActiveUsers = usersInRange.Count(u => u.IsActive),
                InactiveUsers = usersInRange.Count(u => !u.IsActive),
                UsersByRole = usersByRole,
                EmailConfirmedUsers = usersInRange.Count(u => u.EmailConfirmed),
                TwoFactorEnabledUsers = usersInRange.Count(u => u.TwoFactorEnabled)
            };

            return report;
        }

        public async Task<RestaurantReportDto> GetRestaurantReportAsync(DateTime from, DateTime to)
        {
            var allRestaurants = await _restaurantSummaryRepository.GetAllAsync();
            var restaurantsInRange = allRestaurants.Where(r => r.CreatedAt >= from && r.CreatedAt <= to).ToList();

            var restaurantsByOwner = restaurantsInRange
                .GroupBy(r => r.OwnerId)
                .ToDictionary(g => g.Key, g => g.Count());

            var report = new RestaurantReportDto
            {
                FromDate = from,
                ToDate = to,
                TotalRestaurants = restaurantsInRange.Count,
                ActiveRestaurants = restaurantsInRange.Count(r => r.IsActive),
                ApprovedRestaurants = restaurantsInRange.Count(r => r.IsApproved),
                PendingApprovalRestaurants = restaurantsInRange.Count(r => !r.IsApproved),
                AverageRating = restaurantsInRange.Any() ? restaurantsInRange.Average(r => r.Rating) : 0,
                RestaurantsByOwner = restaurantsByOwner
            };

            return report;
        }
    }
}

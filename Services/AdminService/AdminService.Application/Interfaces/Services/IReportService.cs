using AdminService.Application.DTOs.Reports;

namespace AdminService.Application.Interfaces.Services
{
    public interface IReportService
    {
        Task<SalesReportDto> GetSalesReportAsync(DateTime from, DateTime to);
        Task<UserReportDto> GetUserReportAsync(DateTime from, DateTime to);
        Task<RestaurantReportDto> GetRestaurantReportAsync(DateTime from, DateTime to);
    }
}

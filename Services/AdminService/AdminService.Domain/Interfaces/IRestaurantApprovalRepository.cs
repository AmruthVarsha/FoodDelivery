using AdminService.Domain.Entities;

namespace AdminService.Domain.Interfaces
{
    public interface IRestaurantApprovalRepository
    {
        Task AddAsync(RestaurantApprovalRequest request);
        Task<List<RestaurantApprovalRequest>> GetPendingRequestsAsync();
        Task<RestaurantApprovalRequest?> GetByRestaurantIdAsync(Guid restaurantId);
        Task ApproveAsync(Guid restaurantId, string adminId);
        Task RejectAsync(Guid restaurantId, string adminId, string reason);
    }
}

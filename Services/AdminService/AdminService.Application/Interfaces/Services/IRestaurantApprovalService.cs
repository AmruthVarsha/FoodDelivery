using AdminService.Application.DTOs.Restaurants;
using AdminService.Application.DTOs.Users;

namespace AdminService.Application.Interfaces.Services
{
    public interface IRestaurantApprovalService
    {
        Task<List<RestaurantApprovalDto>> GetPendingRestaurantApprovalsAsync();
        Task ApproveRestaurantAsync(Guid restaurantId, string adminId, ApproveRejectDto dto);
        Task RejectRestaurantAsync(Guid restaurantId, string adminId, ApproveRejectDto dto);
    }
}

using AdminService.Application.DTOs.Restaurants;
using AdminService.Application.DTOs.Users;
using AdminService.Application.Exceptions;
using AdminService.Application.Interfaces.Services;
using AdminService.Domain.Interfaces;
using AdminService.Infrastructure.Messaging.Publishers;

namespace AdminService.Application.Services
{
    public class RestaurantApprovalService : IRestaurantApprovalService
    {
        private readonly IRestaurantApprovalRepository _restaurantApprovalRepository;
        private readonly RestaurantApprovalResponsePublisher _publisher;

        public RestaurantApprovalService(
            IRestaurantApprovalRepository restaurantApprovalRepository,
            RestaurantApprovalResponsePublisher publisher)
        {
            _restaurantApprovalRepository = restaurantApprovalRepository;
            _publisher = publisher;
        }

        public async Task<List<RestaurantApprovalDto>> GetPendingRestaurantApprovalsAsync()
        {
            var requests = await _restaurantApprovalRepository.GetPendingRequestsAsync();
            return requests.Select(r => new RestaurantApprovalDto
            {
                RestaurantId = r.RestaurantId,
                OwnerId = r.OwnerId,
                RestaurantName = r.RestaurantName,
                Email = r.Email,
                PhoneNumber = r.PhoneNumber,
                RequestedAt = r.RequestedAt
            }).ToList();
        }

        public async Task ApproveRestaurantAsync(Guid restaurantId, string adminId, ApproveRejectDto dto)
        {
            var request = await _restaurantApprovalRepository.GetByRestaurantIdAsync(restaurantId);
            if (request == null)
                throw new NotFoundException($"Approval request for restaurant {restaurantId} not found");

            await _restaurantApprovalRepository.ApproveAsync(restaurantId, adminId);
            await _publisher.PublishApprovalResponse(restaurantId, true, adminId, null);
        }

        public async Task RejectRestaurantAsync(Guid restaurantId, string adminId, ApproveRejectDto dto)
        {
            var request = await _restaurantApprovalRepository.GetByRestaurantIdAsync(restaurantId);
            if (request == null)
                throw new NotFoundException($"Approval request for restaurant {restaurantId} not found");

            if (string.IsNullOrEmpty(dto.Reason))
                throw new BadRequestException("Rejection reason is required");

            await _restaurantApprovalRepository.RejectAsync(restaurantId, adminId, dto.Reason);
            await _publisher.PublishApprovalResponse(restaurantId, false, adminId, dto.Reason);
        }
    }
}

using OrderService.Application.DTOs.Delivery;
using OrderService.Application.Exceptions;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;
using OrderService.Domain.Interfaces;

namespace OrderService.Application.Services
{
    public class DeliveryAgentProfileService : IDeliveryAgentProfileService
    {
        private readonly IDeliveryAgentProfileRepository _profileRepository;

        public DeliveryAgentProfileService(IDeliveryAgentProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        public async Task<AgentProfileResponseDTO> UpsertProfileAsync(
            string agentUserId, UpsertAgentProfileDTO dto)
        {
            var existing = await _profileRepository.GetByAgentUserId(agentUserId);

            if (existing == null)
            {
                // First-time registration
                var profile = new DeliveryAgentProfile
                {
                    Id = Guid.NewGuid(),
                    AgentUserId = agentUserId,
                    IsActive = dto.IsActive,
                    CurrentPincode = dto.CurrentPincode,
                    LastUpdated = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };
                await _profileRepository.AddAsync(profile);
                return MapToResponse(profile);
            }
            else
            {
                existing.IsActive = dto.IsActive;
                existing.CurrentPincode = dto.CurrentPincode;
                existing.LastUpdated = DateTime.UtcNow;
                await _profileRepository.UpdateAsync(existing);
                return MapToResponse(existing);
            }
        }

        public async Task<AgentProfileResponseDTO> GetProfileAsync(string agentUserId)
        {
            var profile = await _profileRepository.GetByAgentUserId(agentUserId);
            if (profile == null)
                throw new NotFoundException(
                    $"No delivery profile found for agent '{agentUserId}'. Please register first.");

            return MapToResponse(profile);
        }

        private static AgentProfileResponseDTO MapToResponse(DeliveryAgentProfile profile)
        {
            return new AgentProfileResponseDTO
            {
                Id = profile.Id,
                AgentUserId = profile.AgentUserId,
                IsActive = profile.IsActive,
                CurrentPincode = profile.CurrentPincode,
                LastUpdated = profile.LastUpdated
            };
        }
    }
}

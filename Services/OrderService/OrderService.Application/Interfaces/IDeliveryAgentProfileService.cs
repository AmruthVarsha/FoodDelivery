using OrderService.Application.DTOs.Delivery;

namespace OrderService.Application.Interfaces
{
    public interface IDeliveryAgentProfileService
    {
        /// <summary>Create or update an agent's profile (pincode + active status).</summary>
        Task<AgentProfileResponseDTO> UpsertProfileAsync(string agentUserId, UpsertAgentProfileDTO dto);

        Task<AgentProfileResponseDTO> GetProfileAsync(string agentUserId);
    }
}

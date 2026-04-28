using OrderService.Domain.Entities;

namespace OrderService.Domain.Interfaces
{
    public interface IDeliveryAgentProfileRepository
    {
        Task<DeliveryAgentProfile?> GetByAgentUserId(string agentUserId);
        Task<IEnumerable<DeliveryAgentProfile>> GetActiveByPincode(string pincode);
        Task AddAsync(DeliveryAgentProfile profile);
        Task UpdateAsync(DeliveryAgentProfile profile);
    }
}

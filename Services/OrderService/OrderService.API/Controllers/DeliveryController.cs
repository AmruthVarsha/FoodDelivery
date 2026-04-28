using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.DTOs.Delivery;
using OrderService.Application.Interfaces;
using System.Security.Claims;

namespace OrderService.API.Controllers
{
    [ApiController]
    [Route("api/delivery")]
    [Authorize(Roles = "DeliveryAgent")]
    public class DeliveryController : ControllerBase
    {
        private readonly IDeliveryService _deliveryService;

        public DeliveryController(IDeliveryService deliveryService)
        {
            _deliveryService = deliveryService;
        }

        /// <summary>Agent: get their currently assigned orders with full pickup/dropoff details.</summary>
        [HttpGet("assignments")]
        public async Task<IActionResult> GetAssignments()
        {
            var agentId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _deliveryService.GetAssignmentsAsync(agentId);
            return Ok(result);
        }

        /// <summary>
        /// Agent: update delivery status.
        /// Allowed transitions: Assigned → PickedUp → Delivered.
        /// </summary>
        [HttpPut("assignments/{assignmentId:guid}/status")]
        public async Task<IActionResult> UpdateDeliveryStatus(
            Guid assignmentId, [FromBody] UpdateDeliveryStatusDTO dto)
        {
            var agentId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _deliveryService.UpdateDeliveryStatusAsync(assignmentId, agentId, dto);
            return Ok(result);
        }
    }
}

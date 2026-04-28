using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.DTOs.Delivery;
using OrderService.Application.Interfaces;
using System.Security.Claims;

namespace OrderService.API.Controllers
{
    /// <summary>
    /// DeliveryAgent: manage their profile (pincode + active status).
    /// Agents MUST register a profile before they can be auto-assigned orders.
    /// </summary>
    [ApiController]
    [Route("api/delivery/profile")]
    [Authorize(Roles = "DeliveryAgent")]
    public class DeliveryAgentProfileController : ControllerBase
    {
        private readonly IDeliveryAgentProfileService _profileService;

        public DeliveryAgentProfileController(IDeliveryAgentProfileService profileService)
        {
            _profileService = profileService;
        }

        /// <summary>Get current profile.</summary>
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var agentId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _profileService.GetProfileAsync(agentId);
            return Ok(result);
        }

        /// <summary>
        /// Create or update profile. Set IsActive=true to become available for order assignment.
        /// Set IsActive=false to go offline.
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> UpsertProfile([FromBody] UpsertAgentProfileDTO dto)
        {
            var agentId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _profileService.UpsertProfileAsync(agentId, dto);
            return Ok(result);
        }
    }
}

using AdminService.Application.DTOs.Users;
using AdminService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AdminService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class RestaurantApprovalController : ControllerBase
    {
        private readonly IRestaurantApprovalService _restaurantApprovalService;

        public RestaurantApprovalController(IRestaurantApprovalService restaurantApprovalService)
        {
            _restaurantApprovalService = restaurantApprovalService;
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingApprovals()
        {
            var requests = await _restaurantApprovalService.GetPendingRestaurantApprovalsAsync();
            return Ok(requests);
        }

        [HttpPost("{restaurantId:guid}/approve")]
        public async Task<IActionResult> ApproveRestaurant(Guid restaurantId, [FromBody] ApproveRejectDto dto)
        {
            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminId))
                return Unauthorized(new { message = "Admin ID not found" });

            await _restaurantApprovalService.ApproveRestaurantAsync(restaurantId, adminId, dto);
            return Ok(new { message = "Restaurant approved successfully" });
        }

        [HttpPost("{restaurantId:guid}/reject")]
        public async Task<IActionResult> RejectRestaurant(Guid restaurantId, [FromBody] ApproveRejectDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminId))
                return Unauthorized(new { message = "Admin ID not found" });

            await _restaurantApprovalService.RejectRestaurantAsync(restaurantId, adminId, dto);
            return Ok(new { message = "Restaurant rejected successfully" });
        }
    }
}

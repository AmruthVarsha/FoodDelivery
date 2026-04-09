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
    public class UserApprovalController : ControllerBase
    {
        private readonly IUserApprovalService _userApprovalService;

        public UserApprovalController(IUserApprovalService userApprovalService)
        {
            _userApprovalService = userApprovalService;
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingApprovals()
        {
            var requests = await _userApprovalService.GetPendingUserApprovalsAsync();
            return Ok(requests);
        }

        [HttpPost("{email}/approve")]
        public async Task<IActionResult> ApproveUserRole(string email, [FromBody] ApproveRejectDto dto)
        {
            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminId))
                return Unauthorized(new { message = "Admin ID not found" });

            await _userApprovalService.ApproveUserRoleAsync(email, adminId, dto);
            return Ok(new { message = "User role approved successfully" });
        }

        [HttpPost("{email}/reject")]
        public async Task<IActionResult> RejectUserRole(string email, [FromBody] ApproveRejectDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminId))
                return Unauthorized(new { message = "Admin ID not found" });

            await _userApprovalService.RejectUserRoleAsync(email, adminId, dto);
            return Ok(new { message = "User role rejected successfully" });
        }
    }
}

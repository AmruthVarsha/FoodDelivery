using AuthService.Application.DTOs;
using AuthService.Application.Exceptions;
using AuthService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        private string GetIpAddress()
        {
            return HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "Unknown";
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) throw new UnauthorizedException("Access Denied");

            var profile = await _userService.GetProfileAsync(userId);
            return Ok(profile);
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDTO model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) throw new UnauthorizedException("Access Denied");

            await _userService.UpdateProfileAsync(userId, model);
            
            return Ok("Updation Succesfull");
        }

        [Authorize]
        [HttpPut("Deactivate")]
        public async Task<IActionResult> DeactivateAccount()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) throw new UnauthorizedException("Access Denied");

            await _userService.DeactivateAccountAsync(userId, GetIpAddress());
            
            return Ok("Deactivation Succesfull");
        }

        [Authorize]
        [HttpPut("Reactivate")]
        public async Task<IActionResult> ReactivateAccount()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) throw new UnauthorizedException("Access Denied");

            await _userService.ReactivateAccountAsync(userId);
            
            return Ok("Account reactivated successfully");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("AllUsers")]
        public async Task<IActionResult> GetAllUsers([FromQuery] string? role, [FromQuery] bool? isActive)
        {
            var userList = await _userService.GetAllUsersAsync(role, isActive);

            var result = new List<object>();
            foreach (var u in userList)
            {
                var roles = await _userService.GetRolesAsync(u.Id);
                result.Add(new
                {
                    u.Id,
                    u.FullName,
                    u.Email,
                    PhoneNumber = u.PhoneNo,
                    u.IsActive,
                    u.EmailConfirmed,
                    Roles = roles != null && roles.Any() ? string.Join(",", roles) : string.Empty
                });
            }

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            var roles = await _userService.GetRolesAsync(user.Id);

            return Ok(new
            {
                user.Id,
                user.FullName,
                user.Email,
                PhoneNumber = user.PhoneNo,
                user.IsActive,
                user.EmailConfirmed,
                Roles = roles != null && roles.Any() ? string.Join(",", roles) : string.Empty
            });
        }
    }
}

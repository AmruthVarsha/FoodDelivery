using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.DTOs.Order;
using OrderService.Application.Interfaces;
using System.Security.Claims;

namespace OrderService.API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderDTO dto)
        {
            var emailConfirmedClaim = User.FindFirstValue("EmailConfirmed");
            if (emailConfirmedClaim == null || emailConfirmedClaim.ToLower() != "true")
            {
                return StatusCode(403, new { message = "Please confirm your email first before placing an order." });
            }

            var token = Request.Headers["Authorization"].ToString();
            var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _orderService.PlaceOrderAsync(dto, customerId,token);
            return CreatedAtAction(nameof(GetOrderById), new { id = result.Id }, result);
        }

        [HttpGet]
        [Authorize(Roles = "Customer,Partner")]
        public async Task<IActionResult> GetOrderHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var userRole = User.FindFirstValue(ClaimTypes.Role)!;
            var result = await _orderService.GetOrderHistoryAsync(userId, userRole);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _orderService.GetOrderByIdAsync(id, customerId);
            return Ok(result);
        }

        [HttpPut("{id:guid}/cancel")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CancelOrder(Guid id, [FromBody] CancelOrderDTO dto)
        {
            var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _orderService.CancelOrderAsync(id, customerId, dto);
            return Ok("Order cancel succesfully");
        }

        [HttpPut("{id:guid}/status")]
        [Authorize(Roles = "Partner,Admin")]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] UpdateOrderStatusDTO dto)
        {
            await _orderService.UpdateOrderStatusAsync(id, dto);
            return NoContent();
        }
    }
}

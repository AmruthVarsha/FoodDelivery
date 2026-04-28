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

        /// <summary>
        /// Customer: place order from ALL active carts at once.
        /// </summary>
        [HttpPost("checkout")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutDTO dto)
        {
            var emailConfirmed = User.FindFirstValue("EmailConfirmed");
            if (emailConfirmed == null || emailConfirmed.ToLower() != "true")
                return StatusCode(403, new { message = "Please confirm your email before placing an order." });

            var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var customerName = User.FindFirstValue(ClaimTypes.Name)
                            ?? User.FindFirstValue("name")
                            ?? "Customer";
            var token = Request.Headers["Authorization"].ToString();

            var result = await _orderService.CheckoutAsync(dto, customerId, customerName, token);
            return CreatedAtAction(nameof(GetOrderById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Customer: get full order history (all restaurants grouped).
        /// </summary>
        [HttpGet("my")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMyOrders()
        {
            var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _orderService.GetOrderHistoryAsync(customerId);
            return Ok(result);
        }

        /// <summary>
        /// Customer: get a specific order by ID.
        /// </summary>
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _orderService.GetOrderByIdAsync(id, customerId);
            return Ok(result);
        }

        /// <summary>
        /// Customer: cancel their order (within 10 min, before any restaurant accepts).
        /// </summary>
        [HttpPut("{id:guid}/cancel")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CancelOrder(Guid id, [FromBody] CancelOrderDTO dto)
        {
            var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _orderService.CancelOrderAsync(id, customerId, dto);
            return Ok(new { message = "Order cancelled successfully." });
        }
    }
}

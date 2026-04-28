using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.DTOs.Order;
using OrderService.Application.Interfaces;
using OrderService.Domain.Interfaces;
using System.Security.Claims;

namespace OrderService.API.Controllers
{
    /// <summary>
    /// Partner-only endpoints for managing their restaurant's sub-orders.
    /// All operations are automatically scoped to the partner's own restaurant.
    /// </summary>
    [ApiController]
    [Route("api/restaurant-orders")]
    [Authorize(Roles = "Partner")]
    public class RestaurantOrderController : ControllerBase
    {
        private readonly IRestaurantOrderService _restaurantOrderService;
        private readonly ICatalogRepository _catalogRepository;

        public RestaurantOrderController(
            IRestaurantOrderService restaurantOrderService,
            ICatalogRepository catalogRepository)
        {
            _restaurantOrderService = restaurantOrderService;
            _catalogRepository = catalogRepository;
        }

        /// <summary>Partner: get all sub-orders for a specific restaurant they own.</summary>
        [HttpGet("{restaurantId:guid}")]
        public async Task<IActionResult> GetOrdersForRestaurant(Guid restaurantId)
        {
            var partnerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await VerifyRestaurantOwnership(restaurantId, partnerId);

            var result = await _restaurantOrderService.GetOrdersForRestaurantAsync(restaurantId);
            return Ok(result);
        }

        /// <summary>Partner: get a specific sub-order.</summary>
        [HttpGet("{restaurantId:guid}/sub-orders/{subOrderId:guid}")]
        public async Task<IActionResult> GetSubOrder(Guid restaurantId, Guid subOrderId)
        {
            var partnerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await VerifyRestaurantOwnership(restaurantId, partnerId);

            var result = await _restaurantOrderService.GetSubOrderByIdAsync(subOrderId, restaurantId);
            return Ok(result);
        }

        /// <summary>
        /// Partner: update sub-order status.
        /// Allowed transitions: Pending→Accepted, Accepted→Preparing, Preparing→ReadyForPickup.
        /// Also: Rejected (from Pending), Cancelled (from Accepted/Preparing).
        /// </summary>
        [HttpPatch("{restaurantId:guid}/sub-orders/{subOrderId:guid}/status")]
        public async Task<IActionResult> UpdateSubOrderStatus(
            Guid restaurantId,
            Guid subOrderId,
            [FromBody] UpdateRestaurantOrderStatusDTO dto)
        {
            var partnerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await VerifyRestaurantOwnership(restaurantId, partnerId);

            var result = await _restaurantOrderService.UpdateSubOrderStatusAsync(subOrderId, restaurantId, dto);
            return Ok(result);
        }

        private async Task VerifyRestaurantOwnership(Guid restaurantId, string partnerId)
        {
            var isOwner = await _catalogRepository.IsRestaurantOwnedByPartner(restaurantId, partnerId);
            if (!isOwner)
                throw new Application.Exceptions.ForbiddenException(
                    "You do not own this restaurant.");
        }
    }
}

using AdminService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class RestaurantManagementController : ControllerBase
    {
        private readonly IRestaurantManagementService _restaurantManagementService;

        public RestaurantManagementController(IRestaurantManagementService restaurantManagementService)
        {
            _restaurantManagementService = restaurantManagementService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRestaurants()
        {
            var restaurants = await _restaurantManagementService.GetAllRestaurantsAsync();
            return Ok(restaurants);
        }

        [HttpGet("{restaurantId:guid}")]
        public async Task<IActionResult> GetRestaurantById(Guid restaurantId)
        {
            var restaurant = await _restaurantManagementService.GetRestaurantByIdAsync(restaurantId);
            return Ok(restaurant);
        }

        [HttpGet("owner/{ownerId}")]
        public async Task<IActionResult> GetRestaurantsByOwner(string ownerId)
        {
            var restaurants = await _restaurantManagementService.GetRestaurantsByOwnerAsync(ownerId);
            return Ok(restaurants);
        }

        [HttpDelete("{restaurantId:guid}")]
        public async Task<IActionResult> DeleteRestaurant(Guid restaurantId)
        {
            await _restaurantManagementService.DeleteRestaurantAsync(restaurantId);
            return Ok(new { message = "Restaurant deleted successfully" });
        }
    }
}

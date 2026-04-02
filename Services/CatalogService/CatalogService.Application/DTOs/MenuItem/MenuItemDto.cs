namespace CatalogService.Application.DTOs.MenuItem
{
    /// <summary>
    /// Response for:
    ///   GET /menuitems/restaurant/{restaurantId}
    ///   GET /menuitems/category/{categoryId}
    ///   GET /menuitems/{id}
    /// </summary>
    public class MenuItemDto
    {
        public Guid Id { get; set; }
        public Guid RestaurantId { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public bool IsVeg { get; set; }
        public bool IsAvailable { get; set; }
        public int PrepTimeMinutes { get; set; }
    }
}

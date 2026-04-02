namespace CatalogService.Application.DTOs.Category
{
    /// <summary>Response for GET /categories/restaurant/{restaurantId} — menu sidebar.</summary>
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public Guid RestaurantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }
}

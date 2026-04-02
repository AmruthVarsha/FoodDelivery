using CatalogService.Application.DTOs.Restaurant;

namespace CatalogService.Application.DTOs.Restaurant
{
    /// <summary>
    /// Lightweight card response for:
    ///   GET /restaurants
    ///   GET /restaurants/search?q=
    ///   GET /restaurants/nearby?pincode=
    ///   GET /restaurants/cuisine/{cuisineId}
    /// Designed for listing pages — avoids sending full menu data.
    /// </summary>
    public class RestaurantListItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? LogoUrl { get; set; }
        public double Rating { get; set; }
        public int TotalRatings { get; set; }
        public int PrepTimeMinutes { get; set; }
        public bool IsActive { get; set; }
        public string City { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;

        /// <summary>Cuisine names this restaurant serves (e.g., ["Indian", "Chinese"]).</summary>
        public List<string> Cuisines { get; set; } = new();

        /// <summary>Operating hours shown on the card.</summary>
        public string OpeningTime { get; set; } = string.Empty;
        public string ClosingTime { get; set; } = string.Empty;
    }
}

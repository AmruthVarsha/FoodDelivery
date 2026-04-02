namespace CatalogService.Application.DTOs.ServiceArea
{
    /// <summary>Response for GET /serviceareas/{restaurantId} (Partner only).</summary>
    public class ServiceAreaDto
    {
        public Guid Id { get; set; }
        public Guid RestaurantId { get; set; }
        public string Pincode { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}

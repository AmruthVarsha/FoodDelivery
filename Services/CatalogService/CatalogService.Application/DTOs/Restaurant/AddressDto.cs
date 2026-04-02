namespace CatalogService.Application.DTOs.Restaurant
{
    /// <summary>Embedded in RestaurantDetailDto and RestaurantListItemDto responses.</summary>
    public class AddressDto
    {
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
    }
}

namespace CatalogService.Application.DTOs.Cuisine
{
    /// <summary>Response for GET /cuisines — used to populate cuisine chips/tiles on the home page.</summary>
    public class CuisineDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}

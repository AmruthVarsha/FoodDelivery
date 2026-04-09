namespace OrderService.Domain.ExternalDTO
{
    public class CategoryDTO
    {
        public Guid Id { get; set; }
        public Guid RestaurantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}

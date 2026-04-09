namespace AdminService.Application.DTOs.Restaurants
{
    public class RestaurantApprovalDto
    {
        public Guid RestaurantId { get; set; }
        public string OwnerId { get; set; } = string.Empty;
        public string RestaurantName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime RequestedAt { get; set; }
    }
}

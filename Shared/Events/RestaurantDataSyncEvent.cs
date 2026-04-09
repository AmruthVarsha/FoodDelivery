namespace Shared.Events
{
    public class RestaurantDataSyncEvent
    {
        public Guid RestaurantId { get; set; }
        public string OwnerId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public double Rating { get; set; }
        public int TotalRatings { get; set; }
        public bool IsActive { get; set; }
        public bool IsApproved { get; set; }
        public string EventType { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}

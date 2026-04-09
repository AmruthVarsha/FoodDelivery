namespace Shared.Events
{
    public class RestaurantApprovalResponseEvent
    {
        public Guid RestaurantId { get; set; }
        public bool IsApproved { get; set; }
        public string? ApprovedByAdminId { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime ProcessedAt { get; set; }
    }
}

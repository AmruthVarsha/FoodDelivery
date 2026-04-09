using System.ComponentModel.DataAnnotations;

namespace AdminService.Application.DTOs.Users
{
    public class ApproveRejectDto
    {
        [StringLength(500)]
        public string? Reason { get; set; }
    }
}

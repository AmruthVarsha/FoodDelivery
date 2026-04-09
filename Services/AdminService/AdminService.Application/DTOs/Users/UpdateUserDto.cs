using System.ComponentModel.DataAnnotations;

namespace AdminService.Application.DTOs.Users
{
    public class UpdateUserDto
    {
        [Required]
        [StringLength(255)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string PhoneNo { get; set; } = string.Empty;

        [Required]
        public bool IsActive { get; set; }
    }
}

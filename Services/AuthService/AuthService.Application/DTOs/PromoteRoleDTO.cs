using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.DTOs
{
    public class PromoteRoleDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Role name is required")]
        [RegularExpression(@"^(Customer|Partner|DeliveryAgent|Admin)$", ErrorMessage = "Role must be Customer, Partner, DeliveryAgent, or Admin")]
        public string RoleName { get; set; }
    }
}

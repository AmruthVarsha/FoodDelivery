using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.DTOs
{
    public class ConfirmEmailDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "OTP token is required")]
        public string Token { get; set; }
    }
}

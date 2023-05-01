using System.ComponentModel.DataAnnotations;

namespace DOT6_IdentityWeb.Models.DTO
{
    public class RegistrationModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare("Password")]
        [Display (Name = "Password Confirm")]
        public string PasswordConfirm { get; set; }
        public string? Role { get; set; }

    }
}

using System.ComponentModel.DataAnnotations;

namespace be.Dtos.Auth
{
    public class RegisterDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string UserName { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
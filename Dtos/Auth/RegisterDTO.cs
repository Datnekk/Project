using System.ComponentModel.DataAnnotations;

namespace be.Dtos.Auth
{
    public class RegisterDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string UserName { get; set; }
        
        [Required]
        public string Password { get; set; }

    }
}
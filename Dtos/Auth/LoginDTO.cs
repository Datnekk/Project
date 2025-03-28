using System.ComponentModel.DataAnnotations;

namespace be.Dtos.Auth
{
    public class LoginDTO
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }        
    }
}
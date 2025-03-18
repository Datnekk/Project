using System.ComponentModel.DataAnnotations;

namespace be.Dtos.Users
{
    public class UserUpdateDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [MinLength(3, ErrorMessage = "Name must be at least 3 characters long")]
        [MaxLength(15, ErrorMessage = "Name must be at most 15 characters long")]
        public string Name { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }
    }
}
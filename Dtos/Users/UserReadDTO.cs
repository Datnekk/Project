using System.ComponentModel.DataAnnotations;

namespace be.Dtos.Users
{
    public class UserReadDTO
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
    }
}
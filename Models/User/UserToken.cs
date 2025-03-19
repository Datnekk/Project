using System.ComponentModel.DataAnnotations.Schema;

namespace be.Models
{
    public class UserToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpriryTime { get; set; }
    }
}
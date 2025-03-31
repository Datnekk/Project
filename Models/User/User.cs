using System.ComponentModel.DataAnnotations.Schema;
using be.Models.enums;
using Microsoft.AspNetCore.Identity;

namespace be.Models
{
    public class User : IdentityUser<int>
    {
        public string Address { get; set; } = string.Empty;
        public Gender? Gender { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string? Image { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public IList<string> Roles{ get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public ICollection<UserFavorite> Favorites { get; set; } = [];
        public ICollection<Booking>? Bookings { get; set; } = [];
        public ICollection<Room>? Rooms { get; set; } = [];
        public ICollection<Payment>? Payments { get; set; } = [];
    }
}
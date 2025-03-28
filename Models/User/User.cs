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
        
        [Column(TypeName = "nvarchar(max)")]
        public ICollection<UserFavorite> Favorites { get; set; } = [];
        public ICollection<Booking>? Bookings { get; set; } = [];
        public ICollection<Room>? Rooms { get; set; } = [];
        public ICollection<Payment>? Payments { get; set; } = [];
        public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
    }
}
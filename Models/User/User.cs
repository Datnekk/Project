using be.Models.enums;
using Microsoft.AspNetCore.Identity;

namespace be.Models
{
    public class User : IdentityUser<int>
    {
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public Gender? Gender { get; set; }
    public ICollection<Booking>? Bookings { get; set; } = [];
    public ICollection<Room>? Rooms { get; set; } = [];
    public ICollection<Payment>? Payments { get; set; } = [];
    }
}
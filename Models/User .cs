using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace be.Models
{
    public class User : IdentityUser<int>
    {
    [Required, EmailAddress]
    [MaxLength(30)]
    public override string? Email { get; set; }= string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [MaxLength(30)]
    public string? Name { get; set; } = string.Empty;

    [Phone]
    public override string? PhoneNumber { get; set; }

    public ICollection<Booking> Bookings { get; set; }
    }
}
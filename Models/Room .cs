using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using be.Models.enums;

namespace be.Models
{
    public class Room 
    {
    [Key]
    public int RoomId { get; set; }
    public RoomType RoomType { get; set; } 

    [Required]
    public string Location { get; set; }

    public string? Description { get; set; }

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }

    public ICollection<Booking> Bookings { get; set; }
    }
}
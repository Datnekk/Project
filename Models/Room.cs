using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace be.Models
{
    public class Room 
    {
    [Key]
    public int RoomId { get; set; }
    public int UserId { get; set; }
    public string RoomName { get; set; }
    public string Location { get; set; }
    public string? Description { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; } = true;

    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
    public ICollection<RoomService> RoomServices { get; set; } = [];
    }
}
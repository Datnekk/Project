using System.ComponentModel.DataAnnotations.Schema;

namespace be.Models
{
    public class RoomService
    {
    public int RoomId { get; set; }

    [ForeignKey("RoomId")]
    public Room Room { get; set; } = null!;

    public int ServiceId { get; set; }

    [ForeignKey("ServiceId")]
    public Service Service { get; set; } = null!;
    }
}
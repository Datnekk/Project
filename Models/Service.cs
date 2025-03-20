using System.ComponentModel.DataAnnotations;

namespace be.Models
{
    public class Service 
    {
    [Key]
    public int ServiceId { get; set; }
    public string ServiceName { get; set; }
    public string? Description { get; set; }
    public ICollection<RoomService> RoomServices { get; set; } = [];
    }
}
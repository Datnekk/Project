using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using be.Models.enums;

namespace be.Models
{
    public class Booking 
    {
        [Key]
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int RoomId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        [ForeignKey("RoomId")]
        public Room Room { get; set; } = null!;
    }
}
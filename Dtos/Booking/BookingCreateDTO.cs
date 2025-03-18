using System.ComponentModel.DataAnnotations;
using be.Models.enums;

namespace be.Dtos.Booking
{
    public class BookingCreateDTO
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int RoomId { get; set; }

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        public List<int>? BookingServiceIds { get; set; }
    }
}
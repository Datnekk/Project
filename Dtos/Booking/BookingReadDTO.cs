using be.Dtos.Rooms;
using be.Models.enums;

namespace be.Dtos.Booking
{
    public class BookingReadDTO
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int RoomId { get; set; }
        public int TotalPrice { get; set; }
        public DateTime CheckInDate { get; set; }

        public DateTime CheckOutDate { get; set; }

        public DateTime CreatedAt { get; set; }
        
        public BookingStatus Status { get; set; }

        public RoomReadDTO Room { get; set; }
    }
}
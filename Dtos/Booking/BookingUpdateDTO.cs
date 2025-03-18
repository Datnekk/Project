using be.Models.enums;

namespace be.Dtos.Booking
{
    public class BookingUpdateDTO
    {
        public DateTime CheckInDate { get; set; }

        public DateTime CheckOutDate { get; set; }
        
        public BookingStatus Status { get; set; }
    }
}
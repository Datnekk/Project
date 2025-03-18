using be.Models.enums;

namespace be.Helpers
{
    public class BookingQueryObject
    {
        public DateTime? CheckInDate { get; set; }

        public DateTime? CheckOutDate { get; set; }

        public BookingStatus? Status { get; set; }

        public string? SortBy { get; set; }

        public bool IsDecsending { get; set; } = false;
    }
}
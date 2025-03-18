namespace be.Models.enums
{
    public enum BookingStatus
    {
        Pending,      // Waiting for confirmation
        Confirmed,    // Successfully booked
        CheckedIn,    // Guest has checked in
        CheckedOut,   // Guest has checked out
        Cancelled     // Booking has been cancelled
    }
}
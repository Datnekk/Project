namespace be.Models.enums
{
    public enum PaymentStatus
    {
        Pending,    // Payment initiated but not completed
        Completed,  // Payment successfully made
        Failed,     // Payment attempt failed
        Refunded    // Payment refunded
    }
}
namespace be.Models.OTP
{
    public class OtpVerification
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }

}

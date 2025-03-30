using be.Dtos.Auth;

namespace be.Repositories
{
    public interface IOtpService
    {
        Task SaveOtpAsync(string email, string otp);
        Task<bool> VerifyOtpAsync(ConfirmOtpDTO confirmOtpDTO);
    }
}

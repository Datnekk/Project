using be.Data;
using be.Dtos.Auth;
using be.Models.OTP;
using be.Repositories;
using Microsoft.EntityFrameworkCore;

namespace be.Services
{
    public class OtpService : IOtpService
    {
        private readonly ApplicationDbContext _context;

        public OtpService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SaveOtpAsync(string email, string otp)
        {
            var otpEntry = new OtpVerification
            {
                Email = email,
                Otp = otp,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            };

            _context.OtpVerifications.Add(otpEntry);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> VerifyOtpAsync(ConfirmOtpDTO confirmOtpDTO)
        {
            if (confirmOtpDTO == null)
            {
                throw new ArgumentNullException(nameof(confirmOtpDTO), "Request body cannot be null.");
            }

            var otpEntry = await _context.OtpVerifications
                .FirstOrDefaultAsync(o => o.Email == confirmOtpDTO.Email && o.Otp == confirmOtpDTO.Otp);

            if (otpEntry == null)
            {
                return false; // OTP không hợp lệ
            }

            if (otpEntry.ExpiresAt < DateTime.UtcNow)
            {
                _context.OtpVerifications.Remove(otpEntry); // Xóa OTP đã hết hạn
                await _context.SaveChangesAsync();
                return false;
            }

            _context.OtpVerifications.Remove(otpEntry);
            await _context.SaveChangesAsync();

            return true;
        }

    }
}

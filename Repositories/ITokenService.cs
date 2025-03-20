using be.Models;
using Microsoft.IdentityModel.Tokens;

namespace be.Repositories
{
    public interface ITokenService
    {
        Task<string> CreateJWTTokenAsync(User user);
        Task<string> GenerateEmailConfirmationTokenAsync(int userId);
        Task<(bool Succeeded, string[] Errors)> ConfirmEmailAsync(int userId, string token);
        Task<string> GenerateRefreshTokenAsync(int userId);
        Task<(bool Succeeded, string Error)> VerifyRefreshTokenAsync(int userId, string token);
        Task RemoveRefreshTokenAsync(int userId);
        string GetIssuer();
        string GetAudience();
        SymmetricSecurityKey GetKey();
    }
}
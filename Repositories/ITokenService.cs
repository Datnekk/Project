using be.Dtos.Auth;
using be.Models;

namespace be.Repositories
{
    public interface ITokenService
    {
        Task<TokenDTO> CreateJWTTokenAsync(User user, bool populateExp);
        Task<TokenDTO> RefreshJWTTokenAsync(TokenDTO tokenDTO);
        Task<string> GenerateEmailConfirmationTokenAsync(int userId);
        Task<(bool Succeeded, string[] Errors)> ConfirmEmailAsync(int userId, string token);
        void SetTokenCookie(TokenDTO tokenDTO, HttpContext context);
    }
}
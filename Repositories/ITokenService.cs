using be.Models;

namespace be.Repositories
{
    public interface ITokenService
    {
        string CreateToken(User user);
        string GenerateRefreshToken();
        Task<string> GenerateAndSaveRefreshToken(User user);
    }
}
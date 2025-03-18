using be.Models;

namespace be.Repositories
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
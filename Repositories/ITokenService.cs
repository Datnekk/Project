using be.Models;

namespace be.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
using be.Dtos.Users;

namespace be.Repositories;

public interface IUserContext
{
    Task<UserReadDTO?> GetCurrentUserAsync();
    Task<int> GetCurrentUserIdAsync();
    int GetCurrentUserId();
}
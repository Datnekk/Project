namespace be.Repositories;

public interface IUserContext
{
    Task<int> GetCurrentUserIdAsync();
    int GetCurrentUserId();
}
using be.Helpers;
using be.Models;

namespace be.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync(UserQueryObject query);
        Task<User?> GetByIdAsync(int id);
        Task<User?> UpdateAsync(int id, User user);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(int userId, BookingQueryObject query);
        Task<bool> AssignRoleAsync(int id, string role);
    }
}
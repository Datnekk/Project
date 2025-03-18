using be.Helpers;
using be.Models;

namespace be.Repositories
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Booking>> GetAllAsync(BookingQueryObject query);
        Task<Booking?> GetByIdAsync(int id);
        Task<Booking> CreateAsync(Booking booking);
        Task<Booking?> UpdateAsync(int id, Booking booking);
        Task<bool> DeleteAsync(int id);
    }
}
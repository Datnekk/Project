using be.Helpers;
using be.Models;

namespace be.Repositories
{
    public interface IRoomRepository
    {
        Task<IEnumerable<Room>> GetAllAsync(RoomQueryObject query);
        Task<Room?> GetByIdAsync(int id);
        Task<Room> CreateAsync(Room room);
        Task<Room?> UpdateAsync(int id, Room room);
        Task<bool> DeleteAsync(int id);
    }
}
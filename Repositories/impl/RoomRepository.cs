using be.Data;
using be.Helpers;
using be.Interfaces;
using be.Models;
using Microsoft.EntityFrameworkCore;

namespace be.Repositories.impl
{
    public class RoomRepository : IRoomRepository
    {
        private readonly ApplicationDbContext _context;

        public RoomRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Room> CreateAsync(Room room)
        {
            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();
            return room;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(x => x.RoomId == id);

            if (room == null)
                return false;

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Room>> GetAllAsync(RoomQueryObject query)
        {
            var room = _context.Rooms.AsQueryable();

            if (query.Location != null)
                room = room.Where(x => x.Location == query.Location);

            if (query.IsAvailable != null)
                room = room.Where(x => x.IsAvailable == query.IsAvailable);

            if (query.RoomType != null)
                room = room.Where(x => x.RoomType == query.RoomType);

            if (query.MinPrice != null)
                room = room.Where(x => x.Price >= query.MinPrice);

            if (query.MaxPrice != null)
                room = room.Where(x => x.Price <= query.MaxPrice);

            return await room.ToListAsync();  
        }

        public async Task<Room?> GetByIdAsync(int id)
        {
            return await _context.Rooms.FindAsync(id);  
        }

        public async Task<Room?> UpdateAsync(int id, Room room)
        {
            var existingRooms = await _context.Rooms.FirstOrDefaultAsync(x => x.RoomId == id);

            if (existingRooms == null)
                return null;

            _context.Entry(existingRooms).CurrentValues.SetValues(room);
            await _context.SaveChangesAsync();
            return existingRooms;
        }
    }
}
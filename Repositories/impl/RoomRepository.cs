using be.Models;

namespace be.Repositories.impl
{
    public class RoomRepository : IRoomRepository
    {
        private readonly IRepositoryAsync<Room> _roomRepository;

        public RoomRepository(IRepositoryAsync<Room> roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public async Task AddAsync(Room entity, CancellationToken cancellationToken = default)
        {
            await _roomRepository.AddAsync(entity, cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            await _roomRepository.DeleteAsync(id, cancellationToken);
        }

        public async Task<IEnumerable<Room>> GetAsync(
            CancellationToken cancellationToken = default)
        {
            return await _roomRepository.GetAsync(cancellationToken);
        }

        public async Task<Room> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _roomRepository.GetByIdAsync(id, cancellationToken);
        }

        public async Task UpdateAsync(Room entity, CancellationToken cancellationToken = default)
        {
            await _roomRepository.UpdateAsync(entity, cancellationToken);
        }
    }

}
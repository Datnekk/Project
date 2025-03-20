using be.Models;

namespace be.Repositories.impl
{
    public class BookingRepository : IBookingRepository
    {
        private readonly IRepositoryAsync<Booking> _bookingRepository;
        public BookingRepository(IRepositoryAsync<Booking> bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task AddAsync(Booking entity, CancellationToken cancellationToken = default)
        {
            await _bookingRepository.AddAsync(entity, cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            await _bookingRepository.DeleteAsync(id, cancellationToken);
        }

        public async Task<IEnumerable<Booking>> GetAsync(CancellationToken cancellationToken = default)
        {
            return await _bookingRepository.GetAsync(cancellationToken);
        }

        public async Task<Booking> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _bookingRepository.GetByIdAsync(id, cancellationToken);
        }

        public async Task UpdateAsync(Booking entity, CancellationToken cancellationToken = default)
        {
            await _bookingRepository.UpdateAsync(entity, cancellationToken);
        }
    }
}
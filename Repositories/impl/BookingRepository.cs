using be.Data;
using be.Helpers;
using be.Interfaces;
using be.Models;
using Microsoft.EntityFrameworkCore;

namespace be.Repositories.impl
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Booking> CreateAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);

            await _context.SaveChangesAsync();

            return (await _context.Bookings
                                 .Include(b => b.Room)
                                 .FirstOrDefaultAsync(b => b.BookingId == booking.BookingId))!;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(x => x.BookingId == id);

            if (booking == null)
                return false;

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Booking>> GetAllAsync(BookingQueryObject query)
        {
            var booking = _context.Bookings.Include(x => x.Room).AsQueryable();

            if (query.CheckInDate.HasValue)
            {
            booking = booking.Where(b => b.CheckInDate >= query.CheckInDate.Value);
            }

            if (query.CheckOutDate.HasValue)
            {
                booking = booking.Where(b => b.CheckOutDate <= query.CheckOutDate.Value);
            }

            if (query.Status.HasValue)
            {
                booking = booking.Where(b => b.Status == query.Status.Value);
            }

            if(!string.IsNullOrWhiteSpace(query.SortBy))
            {
                if (query.SortBy.ToLower().Equals("checkindate", StringComparison.OrdinalIgnoreCase))
                {
                    booking = query.IsDecsending ? booking.OrderByDescending(b => b.CheckInDate) : booking.OrderBy(b => b.CheckInDate);
                }
                else if (query.SortBy.ToLower() == "checkoutdate")
                {
                    booking = query.IsDecsending ? booking.OrderByDescending(b => b.CheckOutDate) : booking.OrderBy(b => b.CheckOutDate);
                }
                else if (query.SortBy.ToLower() == "status")
                {
                    booking = query.IsDecsending ? booking.OrderByDescending(b => b.Status) : booking.OrderBy(b => b.Status);
                }
            }

            return await booking.ToListAsync();
        }

        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await _context.Bookings
                                 .Include(x => x.Room)
                                 .FirstOrDefaultAsync(x => x.BookingId == id);
        }

        public async Task<Booking?> UpdateAsync(int id, Booking booking)
        {
            var existingBooking = await _context.Bookings.FirstOrDefaultAsync(x => x.BookingId == id);

            if (existingBooking == null)
                return null;

            _context.Entry(existingBooking).CurrentValues.SetValues(booking);
            await _context.SaveChangesAsync();
            return existingBooking;
        }
    }
}
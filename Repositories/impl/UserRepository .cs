using be.Data;
using be.Helpers;
using be.Interfaces;
using be.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace be.Repositories.impl
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;


        public UserRepository(ApplicationDbContext context, UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            _context = context;
            
            _userManager = userManager;

            _roleManager = roleManager;
        }

        public async Task<bool> AssignRoleAsync(int id, string role)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if(user == null)
                return false;

            if (!await _roleManager.RoleExistsAsync(role))
            {
                return false;
            }
            
            var res = await _userManager.AddToRoleAsync(user, role);

            return res.Succeeded;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
                return false;

            var result = await _userManager.DeleteAsync(user);

            return result.Succeeded;
        }

        public async Task<IEnumerable<User>> GetAllAsync(UserQueryObject query)
        {
            var user = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                user = user.Where(x => x.UserName.Contains(query.Name));
            }

            if (!string.IsNullOrWhiteSpace(query.Email))
            {
                user = user.Where(x => x.Email.Contains(query.Email));
            }

            if (!string.IsNullOrWhiteSpace(query.PhoneNumber))
            {
                user = user.Where(x => x.PhoneNumber.Contains(query.PhoneNumber));
            }
            return await user.ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(int userId, BookingQueryObject query)
        {
            var userBookings = _context.Bookings
                .Where(x => x.UserId == userId)
                .Include(x => x.Room)
                .AsQueryable();

            if (query.CheckInDate.HasValue)
            {
                userBookings = userBookings.Where(b => b.CheckInDate >= query.CheckInDate.Value);
            }

            if (query.CheckOutDate.HasValue)
            {
                userBookings = userBookings.Where(b => b.CheckOutDate <= query.CheckOutDate.Value);
            }

            if (query.Status.HasValue)
            {
                userBookings = userBookings.Where(b => b.Status == query.Status.Value);
            }

            return await userBookings.ToListAsync();
        }


        public async Task<User?> GetByIdAsync(int id)
        {
            return await _userManager.FindByIdAsync(id.ToString());
        }

        public async Task<User?> UpdateAsync(int id, User user)
        {
           var existingUser = await _userManager.FindByIdAsync(id.ToString());

           if (existingUser == null)
            return null;

            existingUser.Email = user.Email;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.UserName = user.UserName;

            var result = await _userManager.UpdateAsync(existingUser);

            return result.Succeeded ? existingUser : null;
        }
    }
}
using be.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace be.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<BookingService> BookingServices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            List<IdentityRole<int>> roles = new List<IdentityRole<int>>
            {
                new IdentityRole<int> {Id = 1, Name = "Admin", NormalizedName = "ADMIN"},
                new IdentityRole<int> {Id = 2, Name = "User", NormalizedName = "USER"}
            };
            modelBuilder.Entity<IdentityRole<int>>().HasData(roles);

            // BookingService composite key and relationships
            modelBuilder.Entity<BookingService>()
                .HasKey(bs => new { bs.BookingId, bs.ServiceId });

            modelBuilder.Entity<BookingService>()
                .HasOne(bs => bs.Booking)
                .WithMany(s => s.BookingServices)
                .HasForeignKey(bs => bs.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BookingService>()
                .HasOne(bs => bs.Service)
                .WithMany(s => s.BookingServices)
                .HasForeignKey(bs => bs.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Room-Bookings relationship
            modelBuilder.Entity<Room>()
                .HasMany(r => r.Bookings)
                .WithOne(b => b.Room)
                .HasForeignKey(b => b.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            // Booking-User relationship
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(p => p.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Booking-Room relationship
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Room)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            // Booking-Payment relationship
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Payment)
                .WithOne(p => p.Booking)
                .HasForeignKey<Payment>(p => p.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Booking-BookingService relationship
            modelBuilder.Entity<BookingService>()
                .HasOne(bs => bs.Booking)
                .WithMany(b => b.BookingServices)
                .HasForeignKey(bs => bs.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Booking-User relationship
            modelBuilder.Entity<User>()
                .HasMany(u => u.Bookings)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // UserToken relationship
            modelBuilder.Entity<UserToken>()
                .HasOne(ut  => ut.User)
                .WithMany(u => u.UserTokens)
                .HasForeignKey(ut => ut.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<IdentityUserLogin<int>>().HasKey(x => new { x.LoginProvider, x.ProviderKey });

            modelBuilder.Entity<IdentityUserRole<int>>().HasKey(x => new { x.UserId, x.RoleId });
            
            modelBuilder.Entity<IdentityUserToken<int>>().HasKey(x => new { x.UserId, x.LoginProvider, x.Name });
        }
    }
}
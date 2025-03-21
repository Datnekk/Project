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
        public DbSet<RoomService> RoomServices { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Seed Role for Identity
            List<IdentityRole<int>> roles =
            [
                new IdentityRole<int> {Id = 1, Name = "Admin", NormalizedName = "ADMIN"},
                new IdentityRole<int> {Id = 2, Name = "User", NormalizedName = "USER"},
                new IdentityRole<int> {Id = 3, Name = "LandLord", NormalizedName = "LANDLORD"}
            ];
            modelBuilder.Entity<IdentityRole<int>>().HasData(roles);

            // RoomService (Many-to-Many between Room and Service)

            //Composite key
            modelBuilder.Entity<RoomService>()
                .HasKey(rs => new { rs.RoomId, rs.ServiceId });

            modelBuilder.Entity<RoomService>()
                .HasOne(rs => rs.Room)
                .WithMany(r => r.RoomServices)
                .HasForeignKey(rs => rs.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RoomService>()
                .HasOne(rs => rs.Service)
                .WithMany(s => s.RoomServices)
                .HasForeignKey(rs => rs.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Room-Booking (One-to-One)
            modelBuilder.Entity<Booking>()
                .HasOne(r => r.Room)
                .WithOne()
                .HasForeignKey<Booking>(b => b.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasIndex(b => b.RoomId)
                .IsUnique();

            // User-Room (One-to-Many)
            modelBuilder.Entity<User>()
                .HasMany(u => u.Rooms)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Booking-User (One-to-Many)
            modelBuilder.Entity<User>()
                .HasMany(u => u.Bookings)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Booking-Payment (One-to-One)
            modelBuilder.Entity<Payment>()
                .HasOne(b => b.Booking)
                .WithOne()
                .HasForeignKey<Payment>(p => p.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // User-Payment (One-to-Many)
            modelBuilder.Entity<User>()
                .HasMany(u => u.Payments)
                .WithOne(p => p.User) 
                .HasForeignKey(p => p.UserId) 
                .OnDelete(DeleteBehavior.Restrict);

            // User-RefreshToken (One-to-Many)
            modelBuilder.Entity<User>()
                .HasMany(rt => rt.RefreshTokens)
                .WithOne(rt => rt.User)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            //Composite keys for Identity tables
            modelBuilder.Entity<IdentityUserLogin<int>>().HasKey(x => new { x.LoginProvider, x.ProviderKey });

            modelBuilder.Entity<IdentityUserRole<int>>().HasKey(x => new { x.UserId, x.RoleId });
            
            modelBuilder.Entity<IdentityUserToken<int>>().HasKey(x => new { x.UserId, x.LoginProvider, x.Name });
        }
    }
}
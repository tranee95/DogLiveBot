using DogLiveBot.Data.Context.Configuration;
using DogLiveBot.Data.Context.Entity;
using Microsoft.EntityFrameworkCore;

namespace DogLiveBot.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Dog> Dogs { get; set; }
        public DbSet<UserCallbackQuery> UserCallbackQuerys { get; set; }
        public DbSet<AvailableSlot> AvailableSlots { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Schedule> Schedules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new DogConfiguration());
            modelBuilder.ApplyConfiguration(new UserCallbackQueryConfiguration());
            modelBuilder.ApplyConfiguration(new AvailableSlotConfiguration());
            modelBuilder.ApplyConfiguration(new BookingConfiguration());
            modelBuilder.ApplyConfiguration(new ScheduleConfiguration());
            modelBuilder.ApplyConfiguration(new EventConfiguration());
        }
    }
}
using DogLive.TelegramBot.Data.Context.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Shared.Messages.Repository.Repository.Context;

namespace DogLive.TelegramBot.Data.Context
{
    public class TelegramBotDbContext : ApplicationDbContext
    {
        public TelegramBotDbContext(DbContextOptions<TelegramBotDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Dog> Dogs { get; set; }
        public DbSet<UserCallbackQuery> UserCallbackQueries { get; set; }
        public DbSet<AvailableSlot> AvailableSlots { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Schedule> Schedules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ApplyUtcDateTimeConverters(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TelegramBotDbContext).Assembly);
        }

        private static void ApplyUtcDateTimeConverters(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(UtcDateTimeConverter);
                        property.SetColumnType("timestamp with time zone");
                    }
                    else if (property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(UtcNullableDateTimeConverter);
                        property.SetColumnType("timestamp with time zone");
                    }
                }
            }
        }

        private static readonly ValueConverter<DateTime, DateTime> UtcDateTimeConverter =
            new(
                toProvider => toProvider.Kind == DateTimeKind.Utc ? toProvider : toProvider.ToUniversalTime(),
                fromProvider => DateTime.SpecifyKind(fromProvider, DateTimeKind.Utc)
            );

        private static readonly ValueConverter<DateTime?, DateTime?> UtcNullableDateTimeConverter =
            new(
                toProvider => toProvider == null
                    ? null
                    : (toProvider.Value.Kind == DateTimeKind.Utc ? toProvider : toProvider.Value.ToUniversalTime()),
                fromProvider => fromProvider == null
                    ? null
                    : DateTime.SpecifyKind(fromProvider.Value, DateTimeKind.Utc)
            );
    }
}
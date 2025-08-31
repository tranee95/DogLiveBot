using DogLive.TelegramBot.Data.Context.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DogLive.TelegramBot.Data.Context.Configuration
{
    public class AvailableSlotConfiguration : IEntityTypeConfiguration<AvailableSlot>
    {
        public void Configure(EntityTypeBuilder<AvailableSlot> builder)
        {
            builder.HasKey(s => s.Id);

            builder.HasOne(s => s.Schedule)
                .WithMany(s => s.AvailableSlots)
                .HasForeignKey(s => s.ScheduleId);

            builder.Property(s => s.IsReserved)
                .HasDefaultValue(false);

            builder.Property(s => s.Date)
                .HasColumnType("date");

            builder.HasIndex(s => new { s.ScheduleId, s.DayOfWeek, s.StartTime })
                .IsUnique();

            // Валидация: начало < конец
            builder.ToTable(t => t.HasCheckConstraint("CK_AvailableSlot_TimeRange", "\"StartTime\" < \"EndTime\""));

            // Используем системную колонку xmin для конкуренции (PostgreSQL)
            builder.UseXminAsConcurrencyToken();
        }
    }
}
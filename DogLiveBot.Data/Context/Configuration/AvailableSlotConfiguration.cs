using DogLiveBot.Data.Context.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DogLiveBot.Data.Context.Configuration
{
    public class AvailableSlotConfiguration : IEntityTypeConfiguration<AvailableSlot>
    {
        public void Configure(EntityTypeBuilder<AvailableSlot> builder)
        {
            builder.HasKey(s => s.Id);

            builder.HasOne(s => s.Schedule)
                .WithMany(s => s.AvailableSlots)
                .HasForeignKey(s => s.ScheduleId);

            builder.Property(s => s.Date)
                .HasColumnType("timestamp without time zone");
        }
    }
}
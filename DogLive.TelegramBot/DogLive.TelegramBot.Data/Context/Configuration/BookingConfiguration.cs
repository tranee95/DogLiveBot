using DogLive.TelegramBot.Data.Context.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DogLive.TelegramBot.Data.Context.Configuration
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.HasKey(s => s.Id);

            builder.HasIndex(s => s.TelegramUserId);
            builder.HasIndex(s => s.AvailableSlotId).IsUnique();

            builder.HasIndex(s => new { s.TelegramUserId, s.DogId, s.AvailableSlotId }).IsUnique();

            builder.HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.TelegramUserId)
                .HasPrincipalKey(u => u.TelegramId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.Dog)
                .WithMany()
                .HasForeignKey(s => s.DogId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasOne(s => s.AvailableSlot)
                .WithMany()
                .HasForeignKey(s => s.AvailableSlotId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
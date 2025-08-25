using DogLiveBot.Data.Context.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DogLiveBot.Data.Context.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.TelegramId)
                .IsRequired();

            builder.Property(s => s.PhoneNumber)
                .IsRequired();

            builder.Property(s => s.FirstName)
                .IsRequired();

            builder.Property(s => s.LastName)
                .IsRequired(false);
            
            builder.HasAlternateKey(u => u.TelegramId);

            builder.HasIndex(u => u.TelegramId).IsUnique();
        }
    }
}
using DogLiveBot.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DogLiveBot.Data.Configuration;

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
    }
}
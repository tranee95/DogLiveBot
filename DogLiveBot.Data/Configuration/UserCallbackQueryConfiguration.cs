using DogLiveBot.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DogLiveBot.Data.Configuration;

public class UserCallbackQueryConfiguration : IEntityTypeConfiguration<UserCallbackQuery>
{
    public void Configure(EntityTypeBuilder<UserCallbackQuery> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.CallbackQueryId)
            .IsRequired();

        builder.Property(u => u.Data)
            .IsRequired();

        builder.Property(u => u.ChatId)
            .IsRequired();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(uc => uc.UserTelegramId)
            .HasPrincipalKey(u => u.TelegramId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
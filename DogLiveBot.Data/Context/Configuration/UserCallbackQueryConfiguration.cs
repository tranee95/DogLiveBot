using DogLiveBot.Data.Context.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DogLiveBot.Data.Context.Configuration
{
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

            builder.HasOne(u => u.User)
                .WithOne(u => u.UserCallbackQuery)
                .HasForeignKey<UserCallbackQuery>(s => s.UserTelegramId)
                .HasPrincipalKey<User>(u => u.TelegramId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
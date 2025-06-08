using DogLiveBot.Data.Context.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DogLiveBot.Data.Context.Configuration
{
    public class DogConfiguration : IEntityTypeConfiguration<Dog>
    {
        public void Configure(EntityTypeBuilder<Dog> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Name)
                .IsRequired();

            builder.HasOne(d => d.User)
                .WithMany(u => u.Dogs)
                .HasForeignKey(d => d.UserTelegramId)
                .HasPrincipalKey(u => u.TelegramId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
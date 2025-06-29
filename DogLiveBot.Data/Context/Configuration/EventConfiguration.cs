using DogLiveBot.Data.Context.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DogLiveBot.Data.Context.Configuration;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.EventDate)
            .HasColumnType("timestamp without time zone");

        builder.Property(s => s.StartTime)
            .HasColumnType("timestamp without time zone");

        builder.Property(s => s.EndTime)
            .HasColumnType("timestamp without time zone");

        builder.Property(s => s.CreatedAt)
            .HasColumnType("timestamp without time zone");

        builder.Property(s => s.UpdatedAt)
            .HasColumnType("timestamp without time zone");
    }
}
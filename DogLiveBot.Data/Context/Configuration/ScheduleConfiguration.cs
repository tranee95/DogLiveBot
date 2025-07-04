using DogLiveBot.Data.Context.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DogLiveBot.Data.Context.Configuration;

public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
{
    public void Configure(EntityTypeBuilder<Schedule> builder)
    {
          builder.HasKey(s => s.Id);

          builder.Property(s => s.WeekStartDate)
              .HasColumnType("timestamp without time zone");

          builder.Property(s => s.WeekEndDate)
              .HasColumnType("timestamp without time zone");
    }
}
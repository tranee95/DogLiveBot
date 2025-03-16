using DogLiveBot.Data.Configuration;
using DogLiveBot.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace DogLiveBot.Data.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Dog> Dogs { get; set; }
    public DbSet<UserCallbackQuery> UserCallbackQuerys { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new DogConfiguration());
        modelBuilder.ApplyConfiguration(new UserCallbackQueryConfiguration());
    }
}
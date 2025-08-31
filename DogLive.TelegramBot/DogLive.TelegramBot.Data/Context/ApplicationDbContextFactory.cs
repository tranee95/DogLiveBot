using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DogLive.TelegramBot.Data.Context
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<TelegramBotDbContext>
    {
        public TelegramBotDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TelegramBotDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Port=5499;Database=doglivedb;Username=root;Password=root;");

            return new TelegramBotDbContext(optionsBuilder.Options);
        }
    }
}
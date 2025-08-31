using Microsoft.EntityFrameworkCore;

namespace Shared.Messages.Repository.Repository.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Messages.Repository.Repository.Context;
using Shared.Messages.Repository.Repository.RepositoryImplementations;
using Shared.Messages.Repository.Repository.RepositoryInterfaces;

namespace Shared.Messages.Repository.Repository;

public static class DbContextInjection
{
    public static void Register<TImplementation>(IServiceCollection services, string connectionString)
        where TImplementation : ApplicationDbContext
    {
        services.AddDbContextFactory<TImplementation>(contextOptionsBuilder =>
        {
            contextOptionsBuilder.UseNpgsql(connectionString);
        });
        
        services.AddScoped<IDbContextFactory<ApplicationDbContext>>(sp =>
            new DbContextFactoryWrapper<ApplicationDbContext, TImplementation>(
                sp.GetRequiredService<IDbContextFactory<TImplementation>>()));

        services.AddScoped<IReadOnlyRepository, ApplicationReadOnlyRepository>();
        services.AddScoped<IRepository, ApplicationRepository>();
    }

    public class DbContextFactoryWrapper<TBase, TDerived> : IDbContextFactory<TBase>
        where TDerived : TBase
        where TBase : DbContext
    {
        private readonly IDbContextFactory<TDerived> _innerFactory;

        public DbContextFactoryWrapper(IDbContextFactory<TDerived> innerFactory)
        {
            _innerFactory = innerFactory;
        }

        public TBase CreateDbContext()
        {
            return _innerFactory.CreateDbContext();
        }
    }
}
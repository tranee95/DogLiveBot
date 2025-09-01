using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Messages.Repository.Repository.Context;
using Shared.Messages.Repository.Repository.RepositoryImplementations;
using Shared.Messages.Repository.Repository.RepositoryInterfaces;

namespace Shared.Messages.Repository.Extensions;

public static class DbContextInjection
{
    /// <summary>
    /// Регистрирует контекст базы данных в контейнере зависимостей.
    /// </summary>
    /// <typeparam name="TImplementation">Тип реализации ApplicationDbContext.</typeparam>
    /// <param name="services">Коллекция сервисов для регистрации.</param>
    /// <param name="connectionString">Строка подключения к базе данных.</param>
    public static IServiceCollection RegisterApplicationDbContext<TImplementation>(this IServiceCollection services,
        string connectionString)
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

        return services;
    }

    /// <summary>
    /// Обертка для фабрики контекста базы данных, позволяющая использовать производные типы.
    /// </summary>
    /// <typeparam name="TBase">Базовый тип контекста.</typeparam>
    /// <typeparam name="TDerived">Производный тип контекста.</typeparam>
    private class DbContextFactoryWrapper<TBase, TDerived> : IDbContextFactory<TBase>
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
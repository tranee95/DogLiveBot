using System;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Shared.Messages.Messages.Service.ServiceImplementation;
using Shared.Messages.Messages.Service.ServiceInterface;

namespace Shared.Messages.Messages.Extensions
{
    public static class MassTransitExtensions
    {
        /// <summary>
        /// Регистрация MassTransit в контейнере зависимостей с конфигурацией RabbitMQ.
        /// </summary>
        /// <param name="services">Коллекция сервисов для регистрации.</param>
        /// <param name="rabbitMqHost">Адрес хоста RabbitMQ (по умолчанию "rabbitmq://localhost").</param>
        public static IServiceCollection RegisterMassTransit(this IServiceCollection services,
            string rabbitMqHost = "rabbitmq://localhost")
        {
            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(new Uri(rabbitMqHost));
                    cfg.ConfigureEndpoints(context);
                });
            });

            services.AddScoped<IMassTransitService, MassTransitService>();

            return services;
        }
    }
}
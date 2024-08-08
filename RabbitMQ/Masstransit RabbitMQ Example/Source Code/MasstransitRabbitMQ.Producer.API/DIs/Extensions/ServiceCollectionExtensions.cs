using MassTransit;
using MasstransitRabbitMQ.Producer.API.DIs.Options;

namespace MasstransitRabbitMQ.Producer.API.DIs.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMasstransitRabbitMQConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var masstransitConfiguration = new MasstransitConfiguration();
            configuration.GetSection(nameof(MasstransitConfiguration)).Bind(masstransitConfiguration);

            services.AddMassTransit(masstransit =>
            {
                masstransit.UsingRabbitMq((context, bus) =>
                {
                    bus.Host(masstransitConfiguration.Host, masstransitConfiguration.VHost, host =>
                    {
                        host.Username(masstransitConfiguration.UserName);
                        host.Password(masstransitConfiguration.Password);
                    });
                });
            });

            return services;
        }
    }
}

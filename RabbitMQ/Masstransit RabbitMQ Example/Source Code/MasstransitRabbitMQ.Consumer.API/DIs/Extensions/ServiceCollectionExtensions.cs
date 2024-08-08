using MassTransit;
using MasstransitRabbitMQ.Consumer.API.DIs.Options;
using MasstransitRabbitMQ.Consumer.API.MessageBus.Consumers.Events;
using System.Reflection;

namespace MasstransitRabbitMQ.Consumer.API.DIs.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMasstransitRabbitMQConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var masstransitConfiguration = new MasstransitConfiguration();
            configuration.GetSection(nameof(MasstransitConfiguration)).Bind(masstransitConfiguration);

            services.AddMassTransit(masstransit =>
            {
                //masstransit.AddConsumer<SendSMSWhenReceivedSMSEventConsumer>();

                masstransit.AddConsumers(Assembly.GetExecutingAssembly());

                masstransit.UsingRabbitMq((context, bus) =>
                {
                    bus.Host(masstransitConfiguration.Host, masstransitConfiguration.VHost, host =>
                    {
                        host.Username(masstransitConfiguration.UserName);
                        host.Password(masstransitConfiguration.Password);
                    });

                    bus.ConfigureEndpoints(context);
                });
            });

            return services;
        }

        public static IServiceCollection AddMediaRConfiguration(this IServiceCollection services)
            => services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(Program).Assembly));
    }
}

using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ServiceDefaults.Messaging
{
    public static class MassTransitExtensions
    {
        public static IServiceCollection AddMassTransitWithAssemblies(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddMassTransit(config =>
            {
                // Configure MassTransit with RabbitMQ
                config.SetKebabCaseEndpointNameFormatter();

                // Use in-memory saga repository
                config.SetInMemorySagaRepositoryProvider();

                // Register the consumers
                config.AddConsumers(assemblies);

                // Register the saga state machines
                config.AddSagaStateMachines(assemblies);

                // Register the sagas
                config.AddSagas(assemblies);

                // Register the activities
                config.AddActivities(assemblies);

                config.UsingRabbitMq((context, cfg) =>
                {
                    var configuration = context.GetRequiredService<IConfiguration>();
                    // Get the RabbitMQ connection string from environment variables of the microservice
                    var connectionString = configuration.GetConnectionString("rabbitmq");
                    cfg.Host(connectionString);
                    cfg.ConfigureEndpoints(context);
                });
            });
            // Register the event handlers
            //services.AddScoped<ProductPriceChangedIntegrationEventHandler>();
            return services;
        }
    }
}

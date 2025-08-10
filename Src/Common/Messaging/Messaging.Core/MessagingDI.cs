using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging.Core;

public static class MessagingDI
{
    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration, Assembly? assembly = null)
    {
        var configurationOptions = new MessagingOptions();

        configuration.GetSection(MessagingOptions.SectionName).Bind(configurationOptions);

        services.AddMassTransit(config =>
        {
            config.SetKebabCaseEndpointNameFormatter();

            if (assembly is not null)
            {
                config.AddConsumers(assembly);
            }

            config.UsingRabbitMq((context, configurator) =>
            {
                configurator.Host(new Uri(configurationOptions.Host), host =>
                {
                    host.Username(configurationOptions.Username);
                    host.Password(configurationOptions.Password);
                });
                configurator.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}

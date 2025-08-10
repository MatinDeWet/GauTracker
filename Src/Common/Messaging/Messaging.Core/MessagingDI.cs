using System.Reflection;
using MassTransit;
using Messaging.Core.Abstractions;
using Messaging.Core.MassTransit;
using Messaging.Event.Common.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging.Core;

public static class MessagingDI
{
    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration, Type assemblyPointer)
    {
        ArgumentNullException.ThrowIfNull(assemblyPointer);
        return AddMessaging(services, configuration, assemblyPointer.Assembly);
    }

    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration, params Assembly[] assemblies)
    {
        var options = new MessagingOptions();
        configuration.GetSection(MessagingOptions.SectionName).Bind(options);

        services.AddScoped<IIntegrationEventPublisher, MassTransitIntegrationEventPublisher>();

        HashSet<Type> eventTypes = RegisterIntegrationEventHandlers(services, assemblies);

        services.AddMassTransit(cfg =>
        {
            cfg.SetKebabCaseEndpointNameFormatter();

            // Register adapter consumers for discovered event types only
            foreach (Type evtType in eventTypes)
            {
                Type adapterType = typeof(IntegrationEventConsumerAdapter<>).MakeGenericType(evtType);
                cfg.AddConsumer(adapterType);
            }

            cfg.UsingRabbitMq((context, bus) =>
            {
                bus.Host(new Uri(options.Host), host =>
                {
                    host.Username(options.Username);
                    host.Password(options.Password);
                });

                bus.ConfigureEndpoints(context);
            });
        });

        return services;
    }

    private static HashSet<Type> RegisterIntegrationEventHandlers(IServiceCollection services, Assembly[] assemblies)
    {
        var eventTypes = new HashSet<Type>();

        if (assemblies is null || assemblies.Length == 0)
        {
            return eventTypes;
        }

        services.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(classes => classes.AssignableTo(typeof(IIntegrationEventHandler<>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        foreach (ServiceDescriptor? descriptor in services.Where(d => d.ServiceType.IsGenericType &&
                                                      d.ServiceType.GetGenericTypeDefinition() == typeof(IIntegrationEventHandler<>)))
        {
            Type eventType = descriptor.ServiceType.GetGenericArguments()[0];
            if (typeof(IIntegrationEvent).IsAssignableFrom(eventType))
            {
                eventTypes.Add(eventType);
            }
        }

        return eventTypes;
    }
}

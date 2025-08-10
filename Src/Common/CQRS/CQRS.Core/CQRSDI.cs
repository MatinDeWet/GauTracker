using CQRS.Core.Behaviors;
using CQRS.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace CQRS.Core;
public static class CQRSDI
{
    public static IServiceCollection AddCQRSSupport(this IServiceCollection services, Type assemplyPointer)
    {
        return AddCQRSSupport(services, assemplyPointer, null);
    }

    public static IServiceCollection AddCQRSSupport(this IServiceCollection services, Type assemplyPointer, Action<IServiceCollection>? configureDecorators)
    {
        services.Scan(scan => scan.FromAssembliesOf(assemplyPointer)
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryManager<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandManager<>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandManager<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        // Apply decorators only if there are implementations registered
        TryDecorateIfImplementationsExist(services, typeof(IQueryManager<,>), typeof(LoggingDecorator.QueryManager<,>));
        TryDecorateIfImplementationsExist(services, typeof(ICommandManager<,>), typeof(LoggingDecorator.CommandManager<,>));
        TryDecorateIfImplementationsExist(services, typeof(ICommandManager<>), typeof(LoggingDecorator.CommandBaseManager<>));

        // Apply additional decorators if provided
        configureDecorators?.Invoke(services);

        services.Scan(scan => scan.FromAssembliesOf(assemplyPointer)
            .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventManager<>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }

    /// <summary>
    /// Safely decorates a service type only if implementations are registered for that type.
    /// This prevents DI container errors when no implementations exist.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="serviceType">The service type to decorate (generic type definition)</param>
    /// <param name="decoratorType">The decorator type (generic type definition)</param>
    public static void TryDecorateIfImplementationsExist(this IServiceCollection services, Type serviceType, Type decoratorType)
    {
        // Check if there are any registered implementations for the service type
        bool hasImplementations = services.Any(descriptor =>
            descriptor.ServiceType.IsGenericType &&
            descriptor.ServiceType.GetGenericTypeDefinition() == serviceType);

        if (hasImplementations)
        {
            services.Decorate(serviceType, decoratorType);
        }
    }
}

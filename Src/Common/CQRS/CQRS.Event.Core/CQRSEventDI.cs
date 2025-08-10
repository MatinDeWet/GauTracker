using CQRS.Event.Core.Contracts;
using CQRS.Event.Core.Implementation;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace CQRS.Event.Core;
public static class CQRSEventDI
{
    public static IServiceCollection AddCQRSEventSupport(this IServiceCollection services)
    {
        services.AddTransient<IDomainEventsDispatcher, DomainEventsDispatcher>();

        services.AddScoped<ISaveChangesInterceptor, DomainEventsInterceptor>();

        return services;
    }
}

using Identification.Base;
using Identification.Core.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace Identification.Core;
public static class IdentificationDI
{
    public static IServiceCollection AddIdentitySupport(this IServiceCollection services)
    {
        services.AddScoped<IIdentityInfo, IdentityInfo>();
        services.AddScoped<IInfoSetter, InfoSetter>();

        return services;
    }
}

using Gautrain.Integration.Api;
using Gautrain.Integration.Csv;
using Microsoft.Extensions.DependencyInjection;

namespace Gautrain.Integration.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the Gautrain API client to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddGautrainApi(this IServiceCollection services)
    {
        services.AddScoped<IGautrainApi, GautrainApi>();
        return services;
    }
    /// <summary>
    /// Adds the CSV reader for Gautrain travel history to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddGautrainCsvReader(this IServiceCollection services)
    {
        services.AddScoped<ITravelHistoryCsvReader, TravelHistoryCsvReader>();
        return services;
    }
}

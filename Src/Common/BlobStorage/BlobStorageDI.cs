using BlobStorage.Common.Options;
using BlobStorage.Contracts;
using BlobStorage.Implementation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlobStorage;

public static class BlobStorageDI
{
    public static IServiceCollection AddBlobSupport(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<BlobServiceOptions>(configuration.GetSection(BlobServiceOptions.SectionName));
        services.AddScoped<IBlobService, BlobService>();
        return services;
    }

    public static IServiceCollection AddBlobSupport(this IServiceCollection services, Action<BlobServiceOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddScoped<IBlobService, BlobService>();
        return services;
    }
}

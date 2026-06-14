using WebApi.infrastructure;

namespace WebApi.Presentation.Common.DIExtensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        bool isDevelopmentOrStaging = environment.IsDevelopment() || environment.IsStaging();
        services.AddDatabase(configuration, isDevelopmentOrStaging);

        return services;
    }   
}

using WebApi.infrastructure;

namespace WebApi.Presentation.Common.DIExtensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        bool isDevelopmentOrStaging = environment.IsDevelopment() || environment.IsStaging();

        services.AddApiDocumentation();
        services.AddJwtAuthentication(configuration);
        services.AddDatabase(configuration, isDevelopmentOrStaging);

        return services;
    }   
}

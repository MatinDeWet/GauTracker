using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repository;
using WebApi.infrastructure.Constants;
using WebApi.infrastructure.Data.Contexts;

namespace WebApi.infrastructure;

public static class InfrastructureDI
{
    /// <summary>
    /// Registers the infrastructure layer: the database context and the secured/unsecured
    /// repositories (and their entity protections) discovered in this assembly.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, bool IsDevelopment)
    {
        services.AddDatabase(configuration, IsDevelopment);

        services.AddRepositories(typeof(GauContext));
        services.AddSecuredRepositories(typeof(GauContext));

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration, bool IsDevelopment)
    {
        services.AddDbContext<GauContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

            options.UseNpgsql(
                configuration.GetConnectionString("GauDB"),
                opt =>
                {
                    opt.MigrationsAssembly(typeof(GauContext).GetTypeInfo().Assembly.GetName().Name);
                    opt.MigrationsHistoryTable(HistoryRepository.DefaultTableName, SchemaConstants.Migrations);
                });

            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            if (IsDevelopment)
            {
                options.EnableSensitiveDataLogging();
            }
        });

        return services;
    }
}

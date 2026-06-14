using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApi.infrastructure.Constants;
using WebApi.infrastructure.Data.Contexts;

namespace WebApi.infrastructure;

public static class InfrastructureDI
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration, bool IsDevelopment)
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

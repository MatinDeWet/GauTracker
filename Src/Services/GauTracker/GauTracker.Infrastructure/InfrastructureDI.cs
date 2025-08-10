using System.Reflection;
using GauTracker.Infrastructure.Data.Contexts;
using Infrastructure.Core.Schemas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GauTracker.Infrastructure;
public static class InfrastructureDI
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration, bool IsDevelopment)
    {
        services.AddDbContext<GauTrackerContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                opt =>
                {
                    opt.MigrationsAssembly(typeof(GauTrackerContext).GetTypeInfo().Assembly.GetName().Name);
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

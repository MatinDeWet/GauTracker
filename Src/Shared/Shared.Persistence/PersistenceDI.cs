using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Persistence.Constants;
using Shared.Persistence.Data.Contexts;

namespace Shared.Persistence;

public static class PersistenceDI
{
    /// <summary>
    /// Registers the shared persistence layer: the <see cref="GauContext"/> wired to PostgreSQL.
    /// Both the WebApi and Worker services compose their data access on top of this.
    /// </summary>
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
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

            if (isDevelopment)
            {
                options.EnableSensitiveDataLogging();
            }
        });

        return services;
    }
}

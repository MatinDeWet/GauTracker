using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Background.Core;

public static class BackgroundDI
{
    public static IServiceCollection AddBackgroundClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(configuration => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(
                bootstrapperOptions => bootstrapperOptions.UseNpgsqlConnection(configuration.GetConnectionString("HangfireConnection")!),
                new PostgreSqlStorageOptions
                {
                    QueuePollInterval = TimeSpan.FromSeconds(10),
                    JobExpirationCheckInterval = TimeSpan.FromHours(1),
                    CountersAggregateInterval = TimeSpan.FromMinutes(5),
                    PrepareSchemaIfNecessary = true,
                    TransactionSynchronisationTimeout = TimeSpan.FromMinutes(5),
                    SchemaName = "hangfire"
                }));

        return services;
    }

    public static IServiceCollection AddBackgroundServer(this IServiceCollection services)
    {
        services.AddHangfireServer(options =>
        {
            options.ServerName = "GauTracker-Worker";
            options.WorkerCount = Environment.ProcessorCount;
            options.Queues = ["default"];
            options.ServerTimeout = TimeSpan.FromMinutes(5);
            options.SchedulePollingInterval = TimeSpan.FromSeconds(15);
            options.HeartbeatInterval = TimeSpan.FromSeconds(30);
            options.ServerCheckInterval = TimeSpan.FromMinutes(5);
        });

        return services;
    }
}

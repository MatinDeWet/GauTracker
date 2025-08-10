using Background.Application;
using Background.Infrastructure;
using CQRS.Core;
using CQRS.Event.Core;
using Gautrain.Integration.Extensions;
using Hangfire;
using Hangfire.PostgreSql;
using Observability;
using Repository.Core;

namespace Background.API;

public static class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Host.AddObservability();

        // Hangfire Configuration with PostgreSQL
        builder.Services.AddHangfire(configuration => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(
                bootstrapperOptions => bootstrapperOptions.UseNpgsqlConnection(builder.Configuration.GetConnectionString("HangfireConnection")!),
                new PostgreSqlStorageOptions
                {
                    QueuePollInterval = TimeSpan.FromSeconds(10),
                    JobExpirationCheckInterval = TimeSpan.FromHours(1),
                    CountersAggregateInterval = TimeSpan.FromMinutes(5),
                    PrepareSchemaIfNecessary = true,
                    TransactionSynchronisationTimeout = TimeSpan.FromMinutes(5),
                    SchemaName = "hangfire"
                }));

        // Hangfire Server Configuration
        builder.Services.AddHangfireServer(options =>
        {
            options.ServerName = "GauTracker-Worker";
            options.WorkerCount = Environment.ProcessorCount;
            options.Queues = ["default"];
            options.ServerTimeout = TimeSpan.FromMinutes(5);
            options.SchedulePollingInterval = TimeSpan.FromSeconds(15);
            options.HeartbeatInterval = TimeSpan.FromSeconds(30);
            options.ServerCheckInterval = TimeSpan.FromMinutes(5);
        });

        builder.Services.AddGautrainApi();
        builder.Services.AddGautrainCsvReader();


        builder.Services.AddCQRSSupport(typeof(IApplicationPointer));
        builder.Services.AddCQRSEventSupport();

        builder.Services.AddDatabase(builder.Configuration, builder.Environment.IsDevelopment() || builder.Environment.IsStaging());
        builder.Services.AddRepositories(typeof(IInfrastructurePointer));

        WebApplication app = builder.Build();

        app.UseObservability();

        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            Authorization = [],
            DashboardTitle = "GauTracker Background Jobs",
            DisplayStorageConnectionString = false,
            DarkModeEnabled = true,
            DefaultRecordsPerPage = 20
        });

        app.Run();
    }
}

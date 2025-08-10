using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Observability.Options;
using Serilog;
using Serilog.Events;

namespace Observability;

/// <summary>
/// Extension methods for configuring Serilog
/// </summary>
public static class ObservabilityDI
{
    /// <summary>
    /// Adds Serilog to the host builder
    /// </summary>
    /// <param name="hostBuilder">The host builder</param>
    /// <param name="configureLogger">Optional action to further configure the logger</param>
    /// <returns>The configured host builder</returns>
    public static IHostBuilder AddObservability(this IHostBuilder hostBuilder, Action<LoggerConfiguration>? configureLogger = null)
    {
        hostBuilder.UseSerilog((context, services, loggerConfiguration) =>
        {
            var configurationOptions = new ObservabilityOptions();

            context.Configuration.GetSection(ObservabilityOptions.SectionName).Bind(configurationOptions);

            loggerConfiguration
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information);

            loggerConfiguration
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithProcessId()
                .Enrich.WithProcessName()
                .Enrich.WithThreadId()
                .Enrich.WithEnvironmentName()
                .Enrich.WithProperty("Application", configurationOptions.ApplicationName);

            loggerConfiguration.WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}",
                formatProvider: CultureInfo.InvariantCulture);

            if (context.HostingEnvironment.IsDevelopment())
            {
                loggerConfiguration.WriteTo.Debug(formatProvider: CultureInfo.InvariantCulture);
            }

            ConfigureSink(loggerConfiguration, configurationOptions);

            configureLogger?.Invoke(loggerConfiguration);
        });

        return hostBuilder;
    }

    /// <summary>
    /// Configures OpenObserve sink based on configuration settings
    /// </summary>
    private static void ConfigureSink(LoggerConfiguration loggerConfiguration, ObservabilityOptions configurationOptions)
    {
        if (!Enum.TryParse(configurationOptions.MinimumBreadcrumbLevel, true, out LogEventLevel breadCrumbLevel))
        {
            breadCrumbLevel = LogEventLevel.Information;
        }

        if (!Enum.TryParse(configurationOptions.MinimumEventLevel, true, out LogEventLevel minimumEventLevel))
        {
            minimumEventLevel = LogEventLevel.Error;
        }

        loggerConfiguration.WriteTo.Sentry(x =>
        {
            x.Dsn = configurationOptions.Dns;
            x.MinimumBreadcrumbLevel = breadCrumbLevel;
            x.MinimumEventLevel = minimumEventLevel;
            x.InitializeSdk = true;
        });
    }
}

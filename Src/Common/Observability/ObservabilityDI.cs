using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Observability.Options;
using Serilog;
using Serilog.AspNetCore;
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
    /// Adds Serilog request logging middleware to the application pipeline
    /// </summary>
    /// <param name="app">The application builder</param>
    /// <param name="configureOptions">Optional action to configure request logging options</param>
    /// <returns>The configured application builder</returns>
    public static IApplicationBuilder UseObservability(this IApplicationBuilder app, Action<RequestLoggingOptions>? configureOptions = null)
    {
        if (configureOptions != null)
        {
            app.UseSerilogRequestLogging(configureOptions);
        }
        else
        {
            app.UseSerilogRequestLogging(options =>
            {
                // Customize the message template
                options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

                // Log level configuration
                options.GetLevel = (httpContext, elapsed, ex) => ex != null
                    ? LogEventLevel.Error
                    : elapsed > 1000
                        ? LogEventLevel.Warning
                        : LogEventLevel.Information;

                // Enrich log context with additional properties
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value ?? string.Empty);
                    diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                    diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.FirstOrDefault() ?? string.Empty);

                    if (httpContext.User.Identity?.IsAuthenticated == true)
                    {
                        diagnosticContext.Set("UserId", httpContext.User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(x => x.Value).FirstOrDefault() ?? string.Empty);
                    }
                };
            });
        }

        return app;
    }

    /// <summary>
    /// Configures OpenObserve sink based on configuration settings
    /// </summary>
    private static void ConfigureSink(LoggerConfiguration loggerConfiguration, ObservabilityOptions configurationOptions)
    {
        if (!Enum.TryParse<LogEventLevel>(configurationOptions.MinimumLevel, true, out LogEventLevel logEventLevel))
        {
            logEventLevel = LogEventLevel.Information;
        }

        loggerConfiguration.WriteTo.OpenObserve(
            url: configurationOptions.Url,
            organization: configurationOptions.Organization,
            login: configurationOptions.Login,
            key: configurationOptions.Key,
            streamName: configurationOptions.StreamName,
            restrictedToMinimumLevel: logEventLevel);
    }
}

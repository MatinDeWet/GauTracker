using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Worker.Application;
using Worker.Application.Jobs;
using Worker.infrastructure;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

bool isDevelopmentOrStaging = builder.Environment.IsDevelopment() || builder.Environment.IsStaging();

builder.Services.AddWorkerApplication();
builder.Services.AddWorkerInfrastructure(builder.Configuration, isDevelopmentOrStaging);

IHost host = builder.Build();

// Schedule recurring jobs. (Migrations are owned by the WebApi; this host never applies them.)
IRecurringJobManager recurringJobs = host.Services.GetRequiredService<IRecurringJobManager>();
recurringJobs.AddOrUpdate<IExampleJob>("example-job", job => job.RunAsync(CancellationToken.None), Cron.Hourly());

await host.RunAsync();

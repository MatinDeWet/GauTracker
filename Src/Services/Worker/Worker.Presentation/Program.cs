using Microsoft.Extensions.Hosting;
using Worker.Application;
using Worker.infrastructure;
using Worker.Presentation.Common.DIExtensions;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

bool isDevelopmentOrStaging = builder.Environment.IsDevelopment() || builder.Environment.IsStaging();

builder.Services.AddWorkerApplication();
builder.Services.AddWorkerInfrastructure(builder.Configuration, isDevelopmentOrStaging);

IHost host = builder.Build();

host.RegisterRecurringJobs();

await host.RunAsync();

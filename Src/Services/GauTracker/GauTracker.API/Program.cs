using BlobStorage;
using CQRS.Core;
using CQRS.Event.Core;
using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using GauTracker.API.Common.Filters;
using GauTracker.Application;
using GauTracker.Infrastructure;
using GauTracker.Infrastructure.Data.Contexts;
using Identification.Core;
using Microsoft.EntityFrameworkCore;
using Observability;
using Repository.Core;

namespace GauTracker.API;

public static class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Host.AddObservability();

        builder.Services.AddIdentityPrepration();
        builder.Services.AddIdentitySupport();

        builder.Services.AddCQRSSupport(typeof(IApplicationPointer));
        builder.Services.AddCQRSEventSupport();

        builder.Services.AddDatabase(builder.Configuration, builder.Environment.IsDevelopment() || builder.Environment.IsStaging());
        builder.Services.AddSecuredRepositories(typeof(IInfrastructurePointer));
        builder.Services.AddRepositories(typeof(IInfrastructurePointer));

        builder.Services.AddBlobSupport(builder.Configuration);

        builder.Services
            .AddAuthenticationJwtBearer(o => o.SigningKey = builder.Configuration["Auth:JWTSigningKey"])
            .AddAuthorization()
            .AddFastEndpoints()
            .SwaggerDocument(o =>
            {
                o.AutoTagPathSegmentIndex = 1;
                o.ShortSchemaNames = true;
            });

        WebApplication app = builder.Build();

        app.UseObservability();

        app.UseAuthentication()
           .UseAuthorization()
           .UseFastEndpoints(c =>
           {
               c.Endpoints.Configurator = ep => ep.Options(b => b.AddEndpointFilter<IdentityInfoFilter>());
               c.Endpoints.ShortNames = true;
               c.Endpoints.NameGenerator = ctx =>
               {
                   if (ctx.EndpointType.Name.EndsWith("Endpoint", StringComparison.InvariantCultureIgnoreCase))
                   {
                       return ctx.EndpointType.Name[..^"Endpoint".Length];
                   }
                   else
                   {
                       return ctx.EndpointType.Name;
                   }
               };
           })
           .UseSwaggerGen(uiConfig: u => u.ShowOperationIDs());

        ApplyDbMigrations(app);

        app.Run();
    }

    internal static void ApplyDbMigrations(IApplicationBuilder app)
    {
        using IServiceScope serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()!.CreateScope();

        if (serviceScope.ServiceProvider.GetRequiredService<GauTrackerContext>().Database.GetPendingMigrations().Any())
        {
            serviceScope.ServiceProvider.GetRequiredService<GauTrackerContext>().Database.Migrate();
        }
    }
}

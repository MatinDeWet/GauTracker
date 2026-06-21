using WebApi.Presentation.Common.DIExtensions;
using WebApi.Presentation.Common.Middleware;
using WebApi.Presentation.Endpoints.CardEndpoints;
using WebApi.Presentation.Endpoints.ServiceEndpoints;
using WebApi.Presentation.Endpoints.StationEndpoints;
using WebApi.Presentation.Endpoints.TravelHistoryFileEndpoints;
using WebApi.Presentation.Endpoints.UserEndpoints;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration, builder.Environment);

WebApplication app = builder.Build();

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseApiDocumentation();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<CurrentUserMiddleware>();

app.MapUserEndpoints();
app.MapCardEndpoints();
app.MapTravelHistoryFileEndpoints();
app.MapStationEndpoints();
app.MapServiceEndpoints();

await app.ApplyDatabaseMigrationsAsync();

await app.RunAsync();

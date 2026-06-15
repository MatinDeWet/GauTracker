using WebApi.Presentation.Common.DIExtensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration, builder.Environment);

WebApplication app = builder.Build();

app.UseHttpsRedirection();

// Serve the OpenAPI document and Swagger UI before the auth middleware so the
// secure-by-default fallback policy does not 401 the UI's static assets.
if (app.Environment.IsDevelopment())
{
    app.UseApiDocumentation();
}

app.UseAuthentication();
app.UseAuthorization();

await app.ApplyDatabaseMigrationsAsync();

await app.RunAsync();

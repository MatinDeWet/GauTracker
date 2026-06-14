using WebApi.Presentation.Common.DIExtensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddApplicationServices(builder.Configuration, builder.Environment);

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

await app.ApplyDatabaseMigrationsAsync();

await app.RunAsync();

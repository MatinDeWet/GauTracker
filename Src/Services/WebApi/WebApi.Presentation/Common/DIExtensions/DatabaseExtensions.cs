using Microsoft.EntityFrameworkCore;
using WebApi.infrastructure.Data.Contexts;

namespace WebApi.Presentation.Common.DIExtensions;

public static class DatabaseExtensions
{
    public static async Task ApplyDatabaseMigrationsAsync(this IApplicationBuilder app)
    {
        using IServiceScope serviceScope = app.ApplicationServices
            .GetRequiredService<IServiceScopeFactory>()
            .CreateScope();

        GauContext dbContext = serviceScope.ServiceProvider.GetRequiredService<GauContext>();

        IEnumerable<string> pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

        if (pendingMigrations.Any())
        {
            await dbContext.Database.MigrateAsync();
        }
    }
}

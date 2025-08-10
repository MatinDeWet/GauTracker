using Microsoft.EntityFrameworkCore;

namespace Background.Infrastructure.Data.Contexts;
public class GauTrackerContext : DbContext
{
    public GauTrackerContext() { }

    public GauTrackerContext(DbContextOptions<GauTrackerContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(GauTrackerContext).Assembly);
        builder.ApplyConfigurationsFromAssembly(typeof(SchemaConstants).Assembly);
    }
}

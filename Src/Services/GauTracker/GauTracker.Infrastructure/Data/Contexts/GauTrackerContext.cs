using GauTracker.Domain.Entities;
using Infrastructure.Core.Schemas;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GauTracker.Infrastructure.Data.Contexts;
public class GauTrackerContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
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

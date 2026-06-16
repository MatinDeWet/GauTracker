using Microsoft.EntityFrameworkCore;

namespace WebApi.infrastructure.Data.Contexts;

public class GauContext : DbContext
{
    public GauContext()
    {
        
    }
    
    public GauContext(DbContextOptions<GauContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Trigram extension backing the case-insensitive ILIKE '%...%' searches (e.g. travel-history file names).
        modelBuilder.HasPostgresExtension("pg_trgm");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GauContext).Assembly);
    }
}

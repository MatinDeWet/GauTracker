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
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GauContext).Assembly);
    }
}

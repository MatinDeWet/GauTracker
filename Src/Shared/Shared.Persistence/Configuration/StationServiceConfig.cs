using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Domain.Entities;
using Shared.Persistence.Constants;
using Shared.Persistence.Data.Seeding;

namespace Shared.Persistence.Configuration;

internal sealed class StationServiceConfig : IEntityTypeConfiguration<StationService>
{
    public void Configure(EntityTypeBuilder<StationService> entity)
    {
        entity.ToTable(nameof(StationService), SchemaConstants.Default);

        entity.HasKey(x => new { x.StationId, x.ServiceId });

        entity.HasOne(x => x.Station)
            .WithMany(x => x.StationServices)
            .HasForeignKey(x => x.StationId);

        entity.HasOne(x => x.Service)
            .WithMany(x => x.StationServices)
            .HasForeignKey(x => x.ServiceId);

        entity.HasData(StationServiceSeedData.StationServices);
    }
}

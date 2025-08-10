using Domain.Core.Entities;
using Infrastructure.Core.Schemas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Core.Configuration;
internal partial class StationTransportModeConfiguration : IEntityTypeConfiguration<StationTransportMode>
{
    public void Configure(EntityTypeBuilder<StationTransportMode> entity)
    {
        entity.ToTable(nameof(StationTransportMode), SchemaConstants.Default);

        entity.HasKey(x => new { x.StationId, x.TransportModeId });

        entity.HasOne(x => x.Station)
            .WithMany(x => x.StationTransportModes)
            .HasForeignKey(x => x.StationId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(x => x.TransportMode)
            .WithMany(x => x.StationTransportModes)
            .HasForeignKey(x => x.TransportModeId)
            .OnDelete(DeleteBehavior.Cascade);

        OnConfigurePartial(entity);
    }
    partial void OnConfigurePartial(EntityTypeBuilder<StationTransportMode> entity);
}

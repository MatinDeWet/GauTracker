using Domain.Core.Entities;
using Infrastructure.Core.Schemas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Core.Configuration;
internal partial class StationConfiguration : IEntityTypeConfiguration<Station>
{
    public void Configure(EntityTypeBuilder<Station> entity)
    {
        entity.ToTable(nameof(Station), SchemaConstants.Default);

        entity.HasKey(x => x.Id);

        entity.Property(x => x.ExternalId)
            .IsRequired()
            .HasMaxLength(64);

        entity.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(32);

        entity.Property(x => x.Location)
            .HasColumnType("geography (point)");

        OnConfigurePartial(entity);
    }
    partial void OnConfigurePartial(EntityTypeBuilder<Station> entity);
}

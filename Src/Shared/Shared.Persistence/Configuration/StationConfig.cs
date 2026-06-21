using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Domain.Entities;
using Shared.Domain.Enums;
using Shared.Persistence.Constants;
using Shared.Persistence.Data.Seeding;

namespace Shared.Persistence.Configuration;

internal sealed class StationConfig : IEntityTypeConfiguration<Station>
{
    public void Configure(EntityTypeBuilder<Station> entity)
    {
        entity.ToTable(nameof(Station), SchemaConstants.Default);

        entity.HasKey(x => x.Id);

        entity.Property(x => x.Id)
            .ValueGeneratedNever();

        entity.Property(x => x.Name)
            .HasMaxLength(128)
            .IsRequired();

        entity.Property(x => x.Address)
            .HasMaxLength(256)
            .IsRequired();

        entity.Property(x => x.Latitude)
            .HasPrecision(9, 6);

        entity.Property(x => x.Longitude)
            .HasPrecision(9, 6);

        entity.Property(x => x.StationType)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        // Trigram GIN indexes backing the case-insensitive ILIKE '%term%' search over name + address.
        entity.HasIndex(x => x.Name)
            .HasMethod("gin")
            .HasOperators("gin_trgm_ops");

        entity.HasIndex(x => x.Address)
            .HasMethod("gin")
            .HasOperators("gin_trgm_ops");

        entity.HasMany(x => x.StationServices)
            .WithOne(x => x.Station)
            .HasForeignKey(x => x.StationId);

        entity.HasData(StationSeedData.Stations);
    }
}

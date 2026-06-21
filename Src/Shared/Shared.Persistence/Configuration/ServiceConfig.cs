using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Domain.Entities;
using Shared.Persistence.Constants;
using Shared.Persistence.Data.Seeding;

namespace Shared.Persistence.Configuration;

internal sealed class ServiceConfig : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> entity)
    {
        entity.ToTable(nameof(Service), SchemaConstants.Default);

        entity.HasKey(x => x.Id);

        entity.Property(x => x.Id)
            .ValueGeneratedNever();

        entity.Property(x => x.Name)
            .HasMaxLength(128)
            .IsRequired();

        entity.Property(x => x.Description)
            .HasMaxLength(512)
            .IsRequired();

        // Trigram GIN indexes backing the case-insensitive ILIKE '%term%' search over name + description.
        entity.HasIndex(x => x.Name)
            .HasMethod("gin")
            .HasOperators("gin_trgm_ops");

        entity.HasIndex(x => x.Description)
            .HasMethod("gin")
            .HasOperators("gin_trgm_ops");

        entity.HasMany(x => x.StationServices)
            .WithOne(x => x.Service)
            .HasForeignKey(x => x.ServiceId);

        entity.HasData(ServiceSeedData.Services);
    }
}

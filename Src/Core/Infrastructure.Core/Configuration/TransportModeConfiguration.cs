using Domain.Core.Entities;
using Infrastructure.Core.Schemas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Core.Configuration;
internal partial class TransportModeConfiguration : IEntityTypeConfiguration<TransportMode>
{
    public void Configure(EntityTypeBuilder<TransportMode> entity)
    {
        entity.ToTable(nameof(TransportMode), SchemaConstants.Default);

        entity.HasKey(x => x.Id);

        entity.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(32);

        OnConfigurePartial(entity);
    }
    partial void OnConfigurePartial(EntityTypeBuilder<TransportMode> entity);
}

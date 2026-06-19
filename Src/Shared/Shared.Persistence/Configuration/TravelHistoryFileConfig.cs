using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Domain.Entities;
using Shared.Persistence.Constants;

namespace Shared.Persistence.Configuration;

internal sealed class TravelHistoryFileConfig : IEntityTypeConfiguration<TravelHistoryFile>
{
    public void Configure(EntityTypeBuilder<TravelHistoryFile> entity)
    {
        entity.ToTable(nameof(TravelHistoryFile), SchemaConstants.Default);

        entity.HasKey(x => x.Id);

        entity.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        entity.Property(x => x.FileName)
            .HasMaxLength(256)
            .IsRequired();

        entity.Property(x => x.DisplayName)
            .HasMaxLength(256);

        entity.Property(x => x.ContentType)
            .HasMaxLength(128)
            .IsRequired();

        entity.Property(x => x.BlobContainer)
            .HasMaxLength(63)
            .IsRequired();

        entity.Property(x => x.BlobKey)
            .HasMaxLength(512)
            .IsRequired();

        entity.HasIndex(x => x.BlobKey)
            .IsUnique();

        entity.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        entity.HasOne(x => x.Card)
            .WithMany(x => x.TravelHistoryFiles)
            .HasForeignKey(x => x.CardId);

        // Trigram GIN indexes backing the case-insensitive ILIKE '%term%' search over file name + display name.
        entity.HasIndex(x => x.FileName)
            .HasMethod("gin")
            .HasOperators("gin_trgm_ops");

        entity.HasIndex(x => x.DisplayName)
            .HasMethod("gin")
            .HasOperators("gin_trgm_ops");
    }
}

using Domain.Core.Entities;
using Infrastructure.Core.Schemas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Core.Configuration;
internal partial class TransactionHistoryImportBatchConfiguration : IEntityTypeConfiguration<TransactionHistoryImportBatch>
{
    public void Configure(EntityTypeBuilder<TransactionHistoryImportBatch> entity)
    {
        entity.ToTable(nameof(TransactionHistoryImportBatch), SchemaConstants.Default,
            table => table.HasCheckConstraint("CK_TransactionHistoryImportBatch_Sha256_Hex", "\"Sha256\" ~ '^[0-9A-Fa-f]{64}$'"));

        entity.HasKey(x => x.Id);
        entity.Property(x => x.Id).ValueGeneratedNever();

        entity.Property(x => x.BlobContainer)
            .HasMaxLength(64);

        entity.Property(x => x.BlobName)
            .HasMaxLength(64);

        entity.Property(x => x.Sha256)
            .HasMaxLength(64)
            .IsFixedLength();

        entity.Property(x => x.Error)
            .HasMaxLength(2000);

        entity.Property(x => x.HangfireJobId)
            .HasMaxLength(100);

        entity.HasIndex(x => x.Sha256).IsUnique();
        entity.HasIndex(x => x.Status);
        entity.HasIndex(x => x.UploadedAt);

        entity.Property(x => x.Version).IsRowVersion();

        entity.HasOne(x => x.Card)
            .WithMany(x => x.TransactionHistoryImportBatchs)
            .HasForeignKey(x => x.CardId)
            .OnDelete(DeleteBehavior.Cascade);

        OnConfigurePartial(entity);
    }
    partial void OnConfigurePartial(EntityTypeBuilder<TransactionHistoryImportBatch> entity);
}

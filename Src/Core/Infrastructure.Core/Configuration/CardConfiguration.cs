using Domain.Core.Entities;
using Infrastructure.Core.Schemas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Core.Configuration;
internal partial class CardConfiguration : IEntityTypeConfiguration<Card>
{
    public void Configure(EntityTypeBuilder<Card> entity)
    {
        entity.ToTable(nameof(Card), SchemaConstants.Default);

        entity.HasKey(x => x.Id);

        entity.HasIndex(x => x.Number)
            .IsUnique();

        entity.Property(x => x.Alias)
            .HasMaxLength(64);

        entity.Property(x => x.Number)
            .IsRequired()
            .HasMaxLength(128);

        entity.Property(x => x.ExpiryDate)
            .IsRequired();

        entity.HasOne(x => x.User)
            .WithMany(x => x.Cards)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        OnConfigurePartial(entity);
    }
    partial void OnConfigurePartial(EntityTypeBuilder<Card> entity);
}

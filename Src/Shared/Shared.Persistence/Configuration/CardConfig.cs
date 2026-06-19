using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Domain.Entities;
using Shared.Persistence.Constants;

namespace Shared.Persistence.Configuration;

public class CardConfig: IEntityTypeConfiguration<Card>
{
    public void Configure(EntityTypeBuilder<Card> entity)
    {
        entity.ToTable(nameof(Card), SchemaConstants.Default);
        
        entity.HasKey(x => x.Id);
        
        entity.Property(x => x.Id)
            .ValueGeneratedOnAdd();
        
        entity.Property(x => x.Name)
            .HasMaxLength(64)
            .IsRequired();
        
        entity.Property(x => x.Number)
            .HasMaxLength(64)
            .IsRequired();
        
        entity.HasOne(x => x.User)
            .WithMany(x => x.Cards)
            .HasForeignKey(x => x.UserId);

        entity.HasMany(x => x.TravelHistoryFiles)
            .WithOne(x => x.Card)
            .HasForeignKey(x => x.CardId);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApi.Domain.Entities;
using WebApi.infrastructure.Constants;

namespace WebApi.infrastructure.Configuration;

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
    }
}

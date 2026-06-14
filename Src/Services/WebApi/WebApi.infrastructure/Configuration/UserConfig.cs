using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApi.Domain.Entities;
using WebApi.infrastructure.Constants;

namespace WebApi.infrastructure.Configuration;

internal sealed class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.ToTable(nameof(User), SchemaConstants.Default);
        
        entity.HasKey(x => x.Id);
        
        entity.Property(x => x.Id)
            .ValueGeneratedOnAdd();
        
        entity.HasMany(x => x.Cards)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId);
    }
}

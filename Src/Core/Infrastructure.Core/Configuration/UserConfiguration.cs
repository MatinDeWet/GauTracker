using Domain.Core.Entities;
using Infrastructure.Core.Schemas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Core.Configuration;
internal partial class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.ToTable(nameof(User), SchemaConstants.Default);

        entity.HasKey(x => x.Id);

        entity.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(256);

        OnConfigurePartial(entity);
    }
    partial void OnConfigurePartial(EntityTypeBuilder<User> entity);
}

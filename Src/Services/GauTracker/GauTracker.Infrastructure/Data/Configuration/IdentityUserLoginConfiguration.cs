using Infrastructure.Core.Schemas;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GauTracker.Infrastructure.Data.Configuration;
internal partial class IdentityUserLoginConfiguration : IEntityTypeConfiguration<IdentityUserLogin<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserLogin<Guid>> entity)
    {
        entity.ToTable("AspNetUserLogins", SchemaConstants.Identity);

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<IdentityUserLogin<Guid>> entity);
}

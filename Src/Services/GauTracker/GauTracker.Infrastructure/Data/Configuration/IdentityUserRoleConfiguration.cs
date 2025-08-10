using Infrastructure.Core.Schemas;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GauTracker.Infrastructure.Data.Configuration;
internal partial class IdentityUserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<Guid>> entity)
    {
        entity.ToTable("AspNetUserRoles", SchemaConstants.Identity);

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<IdentityUserRole<Guid>> entity);
}

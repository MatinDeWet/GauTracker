using Infrastructure.Core.Schemas;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GauTracker.Infrastructure.Data.Configuration;
internal partial class IdentityRoleClaimConfiguration : IEntityTypeConfiguration<IdentityRoleClaim<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityRoleClaim<Guid>> entity)
    {
        entity.ToTable("AspNetRoleClaims", SchemaConstants.Identity);

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<IdentityRoleClaim<Guid>> entity);
}

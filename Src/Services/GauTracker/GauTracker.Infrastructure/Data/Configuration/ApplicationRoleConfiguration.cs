using GauTracker.Domain.Entities;
using Infrastructure.Core.Schemas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GauTracker.Infrastructure.Data.Configuration;
internal partial class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> entity)
    {
        entity.ToTable("AspNetRoles", SchemaConstants.Identity);

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ApplicationRole> entity);
}

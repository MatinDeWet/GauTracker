using GauTracker.Domain.Entities;
using Infrastructure.Core.Schemas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GauTracker.Infrastructure.Data.Configuration;
internal partial class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> entity)
    {
        entity.ToTable("AspNetUsers", SchemaConstants.Identity);

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ApplicationUser> entity);
}

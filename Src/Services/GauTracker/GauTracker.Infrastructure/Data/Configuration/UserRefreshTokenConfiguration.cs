using GauTracker.Domain.Entities;
using Infrastructure.Core.Schemas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GauTracker.Infrastructure.Data.Configuration;
internal partial class UserRefreshTokenConfiguration : IEntityTypeConfiguration<UserRefreshToken>
{
    public void Configure(EntityTypeBuilder<UserRefreshToken> entity)
    {
        entity.ToTable(nameof(UserRefreshToken), SchemaConstants.Identity);

        entity.HasKey(x => x.Id);

        entity.HasIndex(x => x.UserID);

        entity.HasIndex(x => x.ExpiryDate)
            .IsDescending(false);

        entity.HasOne(x => x.User)
            .WithMany(x => x.RefreshTokens)
            .HasForeignKey(x => x.UserID)
            .OnDelete(DeleteBehavior.Cascade);

        OnConfigurePartial(entity);
    }
    partial void OnConfigurePartial(EntityTypeBuilder<UserRefreshToken> entity);
}

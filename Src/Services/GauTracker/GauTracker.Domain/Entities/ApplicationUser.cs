using Microsoft.AspNetCore.Identity;

namespace GauTracker.Domain.Entities;
public class ApplicationUser : IdentityUser<Guid>
{
    public virtual ICollection<UserRefreshToken> RefreshTokens { get; private set; } = [];
}

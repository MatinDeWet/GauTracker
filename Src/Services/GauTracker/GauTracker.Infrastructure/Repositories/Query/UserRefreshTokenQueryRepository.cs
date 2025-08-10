using GauTracker.Application.Repositories.Query;
using GauTracker.Domain.Entities;
using GauTracker.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Repository.Core.Implementation;

namespace GauTracker.Infrastructure.Repositories.Query;
internal sealed class UserRefreshTokenQueryRepository : QueryRepo<GauTrackerContext>, IUserRefreshTokenQueryRepository
{
    public UserRefreshTokenQueryRepository(GauTrackerContext context) : base(context)
    {
    }

    public IQueryable<UserRefreshToken> UserRefreshTokens => _context.Set<UserRefreshToken>();

    public async Task<bool> IsValidToken(Guid userId, string token)
    {
        bool isValid = await UserRefreshTokens
            .Where(x => x.UserID == userId
                && x.Token == token
                && x.ExpiryDate >= DateTime.Now
                )
            .AnyAsync();

        return isValid;
    }
}

using GauTracker.Application.Repositories.Command;
using GauTracker.Domain.Entities;
using GauTracker.Infrastructure.Data.Contexts;
using Repository.Core.Implementation;

namespace GauTracker.Infrastructure.Repositories.Command;
internal sealed class UserRefreshTokenCommandRepository : CommandRepo<GauTrackerContext>, IUserRefreshTokenCommandRepository
{
    public UserRefreshTokenCommandRepository(GauTrackerContext context) : base(context)
    {
    }

    public async Task StoreToken(Guid userId, string token, DateTime expirationDate)
    {
        var refreshToken = UserRefreshToken.Create(userId, token, expirationDate);

        await InsertAsync(refreshToken, true, CancellationToken.None);
    }
}

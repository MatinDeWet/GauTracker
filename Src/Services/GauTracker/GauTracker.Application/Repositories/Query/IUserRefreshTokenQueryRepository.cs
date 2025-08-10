using GauTracker.Domain.Entities;
using Repository.Base;

namespace GauTracker.Application.Repositories.Query;
public interface IUserRefreshTokenQueryRepository : IQueryRepo
{
    IQueryable<UserRefreshToken> UserRefreshTokens { get; }

    Task<bool> IsValidToken(Guid userId, string token);
}

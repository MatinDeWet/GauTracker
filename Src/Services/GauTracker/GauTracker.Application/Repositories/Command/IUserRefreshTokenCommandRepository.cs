using Repository.Base;

namespace GauTracker.Application.Repositories.Command;
public interface IUserRefreshTokenCommandRepository : ICommandRepo
{
    Task StoreToken(Guid userId, string token, DateTime expirationDate);
}


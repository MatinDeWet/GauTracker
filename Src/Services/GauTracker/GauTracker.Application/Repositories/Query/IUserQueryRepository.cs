using Domain.Core.Entities;
using Repository.Base;

namespace GauTracker.Application.Repositories.Query;
public interface IUserQueryRepository : ISecureQueryRepo
{
    IQueryable<User> Users { get; }
}

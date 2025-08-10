using Domain.Core.Entities;
using GauTracker.Application.Repositories.Query;
using GauTracker.Infrastructure.Data.Contexts;
using Identification.Base;
using Repository.Core.Contracts;
using Repository.Core.Implementation;

namespace GauTracker.Infrastructure.Repositories.Query;
internal sealed class UserQueryRepository : SecureQueryRepo<GauTrackerContext>, IUserQueryRepository
{
    public UserQueryRepository(GauTrackerContext context, IIdentityInfo info, IEnumerable<IProtected> protection)
        : base(context, info, protection)
    {
    }

    public IQueryable<User> Users => Secure<User>();
}

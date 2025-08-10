using GauTracker.Application.Repositories.Command;
using GauTracker.Infrastructure.Data.Contexts;
using Identification.Base;
using Repository.Core.Contracts;
using Repository.Core.Implementation;

namespace GauTracker.Infrastructure.Repositories.Command;
internal sealed class UserCommandRepository : SecureCommandRepo<GauTrackerContext>, IUserCommandRepository
{
    public UserCommandRepository(GauTrackerContext context, IIdentityInfo info, IEnumerable<IProtected> protection)
        : base(context, info, protection)
    {
    }
}

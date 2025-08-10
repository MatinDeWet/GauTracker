using Background.Application.Repositories.Command;
using Background.Infrastructure.Data.Contexts;
using Repository.Core.Implementation;

namespace Background.Infrastructure.Repositories.Command;
internal sealed class StationCommandRepository : CommandRepo<GauTrackerContext>, IStationCommandRepository
{
    public StationCommandRepository(GauTrackerContext context)
        : base(context)
    {
    }
}

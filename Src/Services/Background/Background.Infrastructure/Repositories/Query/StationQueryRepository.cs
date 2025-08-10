using Background.Application.Repositories.Query;
using Background.Infrastructure.Data.Contexts;
using Domain.Core.Entities;
using Repository.Core.Implementation;

namespace Background.Infrastructure.Repositories.Query;
internal sealed class StationQueryRepository : QueryRepo<GauTrackerContext>, IStationQueryRepository
{
    public StationQueryRepository(GauTrackerContext context)
        : base(context)
    {
    }

    public IQueryable<Station> Stations => GetQueryable<Station>();

    public IQueryable<StationTransportMode> StationTransportModes => GetQueryable<StationTransportMode>();

    public IQueryable<TransportMode> TransportModes => GetQueryable<TransportMode>();
}

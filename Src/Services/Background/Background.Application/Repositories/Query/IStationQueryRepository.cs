using Domain.Core.Entities;
using Repository.Base;

namespace Background.Application.Repositories.Query;
public interface IStationQueryRepository : IQueryRepo
{
    IQueryable<Station> Stations { get; }

    IQueryable<StationTransportMode> StationTransportModes { get; }

    IQueryable<TransportMode> TransportModes { get; }
}

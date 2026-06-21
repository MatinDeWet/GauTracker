using Ardalis.Result;
using CQRS.Core.Contracts;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.Repositories.QueryRepos.UnsecuredRepos;

namespace WebApi.Application.Features.StationFeatures.ListStations;

internal sealed class ListStationsQueryHandler(IUnsecuredQueryRepo queryRepo)
    : IQueryManager<ListStationsQuery, IReadOnlyList<ListStationsResponse>>
{
    public async Task<Result<IReadOnlyList<ListStationsResponse>>> Handle(ListStationsQuery request, CancellationToken cancellationToken)
    {
        List<ListStationsResponse> stations = await queryRepo.Stations
            .Select(x => new ListStationsResponse
            {
                Id = x.Id,
                Name = x.Name,
            })
            .ToListAsync(cancellationToken);

        return stations;
    }
}

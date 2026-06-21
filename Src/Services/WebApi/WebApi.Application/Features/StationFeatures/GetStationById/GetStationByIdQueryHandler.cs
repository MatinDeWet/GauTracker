using Ardalis.Result;
using CQRS.Core.Contracts;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.Repositories.QueryRepos.UnsecuredRepos;

namespace WebApi.Application.Features.StationFeatures.GetStationById;

internal sealed class GetStationByIdQueryHandler(IUnsecuredQueryRepo queryRepo)
    : IQueryManager<GetStationByIdQuery, GetStationByIdResponse>
{
    public async Task<Result<GetStationByIdResponse>> Handle(GetStationByIdQuery request, CancellationToken cancellationToken)
    {
        GetStationByIdResponse? station = await queryRepo.Stations
            .Where(x => x.Id == request.Id)
            .Select(x => new GetStationByIdResponse(
                x.Id,
                x.Name,
                x.Address,
                x.Latitude,
                x.Longitude,
                x.StationType,
                x.IsTerminal))
            .FirstOrDefaultAsync(cancellationToken);

        if (station is null)
        {
            return Result.NotFound();
        }

        return station;
    }
}

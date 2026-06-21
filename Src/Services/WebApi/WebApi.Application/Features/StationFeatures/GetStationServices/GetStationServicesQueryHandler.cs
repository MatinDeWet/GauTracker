using Ardalis.Result;
using CQRS.Core.Contracts;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.Repositories.QueryRepos.UnsecuredRepos;

namespace WebApi.Application.Features.StationFeatures.GetStationServices;

internal sealed class GetStationServicesQueryHandler(IUnsecuredQueryRepo queryRepo)
    : IQueryManager<GetStationServicesQuery, IReadOnlyList<GetStationServicesResponse>>
{
    public async Task<Result<IReadOnlyList<GetStationServicesResponse>>> Handle(GetStationServicesQuery request, CancellationToken cancellationToken)
    {
        List<GetStationServicesResponse>? services = await queryRepo.Stations
            .Where(x => x.Id == request.StationId)
            .Select(x => x.StationServices
                .Select(ss => new GetStationServicesResponse(ss.ServiceId, ss.Service.Name, ss.Service.Description))
                .ToList())
            .FirstOrDefaultAsync(cancellationToken);

        if (services is null)
        {
            return Result.NotFound();
        }

        return services;
    }
}

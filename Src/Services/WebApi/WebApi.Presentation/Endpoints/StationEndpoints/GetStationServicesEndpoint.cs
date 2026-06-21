using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using CQRS.Core.Contracts;
using Microsoft.AspNetCore.Mvc;
using WebApi.Application.Features.StationFeatures.GetStationServices;

namespace WebApi.Presentation.Endpoints.StationEndpoints;

internal static class GetStationServicesEndpoint
{
    public static RouteGroupBuilder MapGetStationServicesEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:int}/services", GetStationServices)
            .WithName("GetStationServices")
            .WithSummary("Returns the services available at a station.");

        return group;
    }

    private static async Task<Microsoft.AspNetCore.Http.IResult> GetStationServices(
        [FromRoute] int id,
        [FromServices] IQueryManager<GetStationServicesQuery, IReadOnlyList<GetStationServicesResponse>> handler,
        CancellationToken cancellationToken)
    {
        Result<IReadOnlyList<GetStationServicesResponse>> result = await handler.Handle(new GetStationServicesQuery(id), cancellationToken);

        return result.ToMinimalApiResult();
    }
}

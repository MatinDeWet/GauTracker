using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using CQRS.Core.Contracts;
using Microsoft.AspNetCore.Mvc;
using WebApi.Application.Features.StationFeatures.GetStationById;

namespace WebApi.Presentation.Endpoints.StationEndpoints;

internal static class GetStationByIdEndpoint
{
    public static RouteGroupBuilder MapGetStationByIdEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:int}", GetStationById)
            .WithName("GetStationById")
            .WithSummary("Returns a single station.");

        return group;
    }

    private static async Task<Microsoft.AspNetCore.Http.IResult> GetStationById(
        [FromRoute] int id,
        [FromServices] IQueryManager<GetStationByIdQuery, GetStationByIdResponse> handler,
        CancellationToken cancellationToken)
    {
        Result<GetStationByIdResponse> result = await handler.Handle(new GetStationByIdQuery(id), cancellationToken);

        return result.ToMinimalApiResult();
    }
}

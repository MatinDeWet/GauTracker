using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using CQRS.Core.Contracts;
using Microsoft.AspNetCore.Mvc;
using WebApi.Application.Features.StationFeatures.ListStations;

namespace WebApi.Presentation.Endpoints.StationEndpoints;

internal static class ListStationsEndpoint
{
    public static RouteGroupBuilder MapListStationsEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/", ListStations)
            .WithName("ListStations")
            .WithSummary("Returns all stations (id and name).");

        return group;
    }

    private static async Task<Microsoft.AspNetCore.Http.IResult> ListStations(
        [FromServices] IQueryManager<ListStationsQuery, IReadOnlyList<ListStationsResponse>> handler,
        CancellationToken cancellationToken)
    {
        Result<IReadOnlyList<ListStationsResponse>> result = await handler.Handle(new ListStationsQuery(), cancellationToken);

        return result.ToMinimalApiResult();
    }
}

using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using CQRS.Core.Contracts;
using Microsoft.AspNetCore.Mvc;
using WebApi.Application.Features.TravelHistoryFileFeatures.GetTravelHistoryFileById;

namespace WebApi.Presentation.Endpoints.TravelHistoryFileEndpoints;

internal static class GetTravelHistoryFileByIdEndpoint
{
    public static RouteGroupBuilder MapGetTravelHistoryFileByIdEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:long}", GetTravelHistoryFileById)
            .WithName("GetTravelHistoryFileById")
            .WithSummary("Returns a single travel-history file's metadata.");

        return group;
    }

    private static async Task<Microsoft.AspNetCore.Http.IResult> GetTravelHistoryFileById(
        [FromRoute] long id,
        [FromServices] IQueryManager<GetTravelHistoryFileByIdQuery, GetTravelHistoryFileByIdResponse> handler,
        CancellationToken cancellationToken)
    {
        Result<GetTravelHistoryFileByIdResponse> result = await handler.Handle(new GetTravelHistoryFileByIdQuery(id), cancellationToken);

        return result.ToMinimalApiResult();
    }
}

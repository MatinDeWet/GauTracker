using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using CQRS.Core.Contracts;
using Microsoft.AspNetCore.Mvc;
using WebApi.Application.Features.TravelHistoryFileFeatures.GetTravelHistoryFileDownloadUrl;

namespace WebApi.Presentation.Endpoints.TravelHistoryFileEndpoints;

internal static class GetTravelHistoryFileDownloadUrlEndpoint
{
    public static RouteGroupBuilder MapGetTravelHistoryFileDownloadUrlEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:long}/download-url", GetTravelHistoryFileDownloadUrl)
            .WithName("GetTravelHistoryFileDownloadUrl")
            .WithSummary("Returns a presigned download URL for a travel-history file.");

        return group;
    }

    private static async Task<Microsoft.AspNetCore.Http.IResult> GetTravelHistoryFileDownloadUrl(
        [FromRoute] long id,
        [FromServices] IQueryManager<GetTravelHistoryFileDownloadUrlQuery, GetTravelHistoryFileDownloadUrlResponse> handler,
        CancellationToken cancellationToken)
    {
        Result<GetTravelHistoryFileDownloadUrlResponse> result = await handler.Handle(new GetTravelHistoryFileDownloadUrlQuery(id), cancellationToken);

        return result.ToMinimalApiResult();
    }
}

using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using CQRS.Core.Contracts;
using Microsoft.AspNetCore.Mvc;
using WebApi.Application.Features.TravelHistoryFileFeatures.GetTravelHistoryFileUploadUrl;

namespace WebApi.Presentation.Endpoints.TravelHistoryFileEndpoints;

internal static class GetTravelHistoryFileUploadUrlEndpoint
{
    public static RouteGroupBuilder MapGetTravelHistoryFileUploadUrlEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:long}/upload-url", GetTravelHistoryFileUploadUrl)
            .WithName("GetTravelHistoryFileUploadUrl")
            .WithSummary("Issues a fresh presigned upload URL for an existing travel-history file.");

        return group;
    }

    private static async Task<Microsoft.AspNetCore.Http.IResult> GetTravelHistoryFileUploadUrl(
        [FromRoute] long id,
        [FromServices] IQueryManager<GetTravelHistoryFileUploadUrlQuery, GetTravelHistoryFileUploadUrlResponse> handler,
        CancellationToken cancellationToken)
    {
        Result<GetTravelHistoryFileUploadUrlResponse> result = await handler.Handle(new GetTravelHistoryFileUploadUrlQuery(id), cancellationToken);

        return result.ToMinimalApiResult();
    }
}

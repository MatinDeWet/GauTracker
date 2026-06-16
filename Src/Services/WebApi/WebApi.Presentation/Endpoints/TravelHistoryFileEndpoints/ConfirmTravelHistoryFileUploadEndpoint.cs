using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using CQRS.Core.Contracts;
using Microsoft.AspNetCore.Mvc;
using WebApi.Application.Features.TravelHistoryFileFeatures.ConfirmTravelHistoryFileUpload;

namespace WebApi.Presentation.Endpoints.TravelHistoryFileEndpoints;

internal static class ConfirmTravelHistoryFileUploadEndpoint
{
    public static RouteGroupBuilder MapConfirmTravelHistoryFileUploadEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/{id:long}/confirm-upload", ConfirmTravelHistoryFileUpload)
            .WithName("ConfirmTravelHistoryFileUpload")
            .WithSummary("Confirms a presigned upload completed, recording the file size.");

        return group;
    }

    private static async Task<Microsoft.AspNetCore.Http.IResult> ConfirmTravelHistoryFileUpload(
        [FromRoute] long id,
        [FromServices] ICommandManager<ConfirmTravelHistoryFileUploadCommand> handler,
        CancellationToken cancellationToken)
    {
        Result result = await handler.Handle(new ConfirmTravelHistoryFileUploadCommand(id), cancellationToken);

        return result.ToMinimalApiResult();
    }
}

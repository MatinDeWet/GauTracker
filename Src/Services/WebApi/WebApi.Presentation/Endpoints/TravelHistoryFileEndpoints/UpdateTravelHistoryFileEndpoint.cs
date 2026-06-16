using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using CQRS.Core.Contracts;
using Microsoft.AspNetCore.Mvc;
using WebApi.Application.Features.TravelHistoryFileFeatures.UpdateTravelHistoryFile;

namespace WebApi.Presentation.Endpoints.TravelHistoryFileEndpoints;

internal static class UpdateTravelHistoryFileEndpoint
{
    public static RouteGroupBuilder MapUpdateTravelHistoryFileEndpoint(this RouteGroupBuilder group)
    {
        group.MapPut("/{id:long}", UpdateTravelHistoryFile)
            .WithName("UpdateTravelHistoryFile")
            .WithSummary("Updates a travel-history file's name / display name (not the stored blob).");

        return group;
    }

    private static async Task<Microsoft.AspNetCore.Http.IResult> UpdateTravelHistoryFile(
        [FromRoute] long id,
        [FromBody] UpdateTravelHistoryFileRequest request,
        [FromServices] ICommandManager<UpdateTravelHistoryFileCommand> handler,
        CancellationToken cancellationToken)
    {
        UpdateTravelHistoryFileCommand command = new(id, request.FileName, request.DisplayName);

        Result result = await handler.Handle(command, cancellationToken);

        return result.ToMinimalApiResult();
    }

    private sealed record UpdateTravelHistoryFileRequest(string FileName, string? DisplayName);
}

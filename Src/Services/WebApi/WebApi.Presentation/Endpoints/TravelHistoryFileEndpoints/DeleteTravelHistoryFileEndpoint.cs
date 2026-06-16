using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using CQRS.Core.Contracts;
using Microsoft.AspNetCore.Mvc;
using WebApi.Application.Features.TravelHistoryFileFeatures.DeleteTravelHistoryFile;

namespace WebApi.Presentation.Endpoints.TravelHistoryFileEndpoints;

internal static class DeleteTravelHistoryFileEndpoint
{
    public static RouteGroupBuilder MapDeleteTravelHistoryFileEndpoint(this RouteGroupBuilder group)
    {
        group.MapDelete("/{id:long}", DeleteTravelHistoryFile)
            .WithName("DeleteTravelHistoryFile")
            .WithSummary("Deletes a travel-history file and its stored blob.");

        return group;
    }

    private static async Task<Microsoft.AspNetCore.Http.IResult> DeleteTravelHistoryFile(
        [FromRoute] long id,
        [FromServices] ICommandManager<DeleteTravelHistoryFileCommand> handler,
        CancellationToken cancellationToken)
    {
        Result result = await handler.Handle(new DeleteTravelHistoryFileCommand(id), cancellationToken);

        return result.ToMinimalApiResult();
    }
}

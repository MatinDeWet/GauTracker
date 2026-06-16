using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using CQRS.Core.Contracts;
using Microsoft.AspNetCore.Mvc;
using WebApi.Application.Features.TravelHistoryFileFeatures.CreateTravelHistoryFile;

namespace WebApi.Presentation.Endpoints.TravelHistoryFileEndpoints;

internal static class CreateTravelHistoryFileEndpoint
{
    public static RouteGroupBuilder MapCreateTravelHistoryFileEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/", CreateTravelHistoryFile)
            .WithName("CreateTravelHistoryFile")
            .WithSummary("Creates a travel-history file under a card and returns a presigned upload URL.");

        return group;
    }

    private static async Task<Microsoft.AspNetCore.Http.IResult> CreateTravelHistoryFile(
        [FromRoute] long cardId,
        [FromBody] CreateTravelHistoryFileRequest request,
        [FromServices] ICommandManager<CreateTravelHistoryFileCommand, CreateTravelHistoryFileResponse> handler,
        CancellationToken cancellationToken)
    {
        CreateTravelHistoryFileCommand command = new(cardId, request.FileName, request.ContentType, request.DisplayName);

        Result<CreateTravelHistoryFileResponse> result = await handler.Handle(command, cancellationToken);

        return result.ToMinimalApiResult();
    }

    private sealed record CreateTravelHistoryFileRequest(string FileName, string? ContentType, string? DisplayName);
}

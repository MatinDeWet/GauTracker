using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using CQRS.Core.Contracts;
using Microsoft.AspNetCore.Mvc;
using WebApi.Application.Features.CardFeatures.UpdateCard;

namespace WebApi.Presentation.Endpoints.CardEndpoints;

internal static class UpdateCardEndpoint
{
    public static RouteGroupBuilder MapUpdateCardEndpoint(this RouteGroupBuilder group)
    {
        group.MapPut("/{id:long}", UpdateCard)
            .WithName("UpdateCard")
            .WithSummary("Updates a card owned by the current user.");

        return group;
    }

    private static async Task<Microsoft.AspNetCore.Http.IResult> UpdateCard(
        [FromRoute] long id,
        [FromBody] UpdateCardRequest request,
        [FromServices] ICommandManager<UpdateCardCommand> handler,
        CancellationToken cancellationToken)
    {
        UpdateCardCommand command = new(id, request.Name, request.Number, request.ExpiryDate);

        Result result = await handler.Handle(command, cancellationToken);

        return result.ToMinimalApiResult();
    }

    private sealed record UpdateCardRequest(string Name, string Number, DateOnly ExpiryDate);
}

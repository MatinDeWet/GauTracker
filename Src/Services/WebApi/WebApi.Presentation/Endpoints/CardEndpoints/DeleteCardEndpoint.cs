using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using CQRS.Core.Contracts;
using WebApi.Application.Features.CardFeatures.DeleteCard;

namespace WebApi.Presentation.Endpoints.CardEndpoints;

internal static class DeleteCardEndpoint
{
    public static RouteGroupBuilder MapDeleteCardEndpoint(this RouteGroupBuilder group)
    {
        group.MapDelete("/{id:long}", DeleteCard)
            .WithName("DeleteCard")
            .WithSummary("Deletes a card owned by the current user.");

        return group;
    }

    private static async Task<Microsoft.AspNetCore.Http.IResult> DeleteCard(
        long id,
        ICommandManager<DeleteCardCommand> handler,
        CancellationToken cancellationToken)
    {
        Result result = await handler.Handle(new DeleteCardCommand(id), cancellationToken);

        return result.ToMinimalApiResult();
    }
}

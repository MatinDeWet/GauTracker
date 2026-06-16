using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using CQRS.Core.Contracts;
using Microsoft.AspNetCore.Mvc;
using WebApi.Application.Features.CardFeatures.CreateCard;

namespace WebApi.Presentation.Endpoints.CardEndpoints;

internal static class CreateCardEndpoint
{
    public static RouteGroupBuilder MapCreateCardEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/", CreateCard)
            .WithName("CreateCard")
            .WithSummary("Creates a new card for the current user.");

        return group;
    }

    private static async Task<Microsoft.AspNetCore.Http.IResult> CreateCard(
        [FromBody] CreateCardCommand command,
        [FromServices] ICommandManager<CreateCardCommand, CreateCardResponse> handler,
        CancellationToken cancellationToken)
    {
        Result<CreateCardResponse> result = await handler.Handle(command, cancellationToken);

        return result.ToMinimalApiResult();
    }
}

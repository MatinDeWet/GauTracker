using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using CQRS.Core.Contracts;
using Microsoft.AspNetCore.Mvc;
using WebApi.Application.Features.CardFeatures.GetCardById;

namespace WebApi.Presentation.Endpoints.CardEndpoints;

internal static class GetCardByIdEndpoint
{
    public static RouteGroupBuilder MapGetCardByIdEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:long}", GetCardById)
            .WithName("GetCardById")
            .WithSummary("Returns a single card owned by the current user.");

        return group;
    }

    private static async Task<Microsoft.AspNetCore.Http.IResult> GetCardById(
        [FromRoute] long id,
        [FromServices] IQueryManager<GetCardByIdQuery, GetCardByIdResponse> handler,
        CancellationToken cancellationToken)
    {
        Result<GetCardByIdResponse> result = await handler.Handle(new GetCardByIdQuery(id), cancellationToken);

        return result.ToMinimalApiResult();
    }
}

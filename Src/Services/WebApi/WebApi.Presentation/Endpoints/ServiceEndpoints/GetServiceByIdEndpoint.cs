using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using CQRS.Core.Contracts;
using Microsoft.AspNetCore.Mvc;
using WebApi.Application.Features.ServiceFeatures.GetServiceById;

namespace WebApi.Presentation.Endpoints.ServiceEndpoints;

internal static class GetServiceByIdEndpoint
{
    public static RouteGroupBuilder MapGetServiceByIdEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:int}", GetServiceById)
            .WithName("GetServiceById")
            .WithSummary("Returns a single service.");

        return group;
    }

    private static async Task<Microsoft.AspNetCore.Http.IResult> GetServiceById(
        [FromRoute] int id,
        [FromServices] IQueryManager<GetServiceByIdQuery, GetServiceByIdResponse> handler,
        CancellationToken cancellationToken)
    {
        Result<GetServiceByIdResponse> result = await handler.Handle(new GetServiceByIdQuery(id), cancellationToken);

        return result.ToMinimalApiResult();
    }
}

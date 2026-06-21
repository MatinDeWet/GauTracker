using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using CQRS.Core.Contracts;
using Microsoft.AspNetCore.Mvc;
using WebApi.Application.Features.ServiceFeatures.ListServices;

namespace WebApi.Presentation.Endpoints.ServiceEndpoints;

internal static class ListServicesEndpoint
{
    public static RouteGroupBuilder MapListServicesEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/", ListServices)
            .WithName("ListServices")
            .WithSummary("Returns all services (id, name and description).");

        return group;
    }

    private static async Task<Microsoft.AspNetCore.Http.IResult> ListServices(
        [FromServices] IQueryManager<ListServicesQuery, IReadOnlyList<ListServicesResponse>> handler,
        CancellationToken cancellationToken)
    {
        Result<IReadOnlyList<ListServicesResponse>> result = await handler.Handle(new ListServicesQuery(), cancellationToken);

        return result.ToMinimalApiResult();
    }
}

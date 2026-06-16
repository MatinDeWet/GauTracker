using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using CQRS.Core.Contracts;
using Microsoft.AspNetCore.Mvc;
using Pagination.Enums;
using Pagination.Models.Responses;
using WebApi.Application.Features.CardFeatures.SearchCards;

namespace WebApi.Presentation.Endpoints.CardEndpoints;

internal static class SearchCardsEndpoint
{
    public static RouteGroupBuilder MapSearchCardsEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/", SearchCards)
            .WithName("SearchCards")
            .WithSummary("Returns a paginated list of the current user's cards.");

        return group;
    }

    private static async Task<Microsoft.AspNetCore.Http.IResult> SearchCards(
        [AsParameters] SearchCardsRequest request,
        [FromServices] IQueryManager<SearchCardsQuery, PageableResponse<SearchCardsResponse>> handler,
        CancellationToken cancellationToken)
    {
        SearchCardsQuery query = new()
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            OrderBy = request.OrderBy,
            OrderDirection = request.OrderDirection,
        };

        Result<PageableResponse<SearchCardsResponse>> result = await handler.Handle(query, cancellationToken);

        return result.ToMinimalApiResult();
    }

    /// <summary>Query-string parameters for <see cref="SearchCards"/>; all optional with defaults.</summary>
    private readonly record struct SearchCardsRequest(
        [property: FromQuery] int PageNumber = 1,
        [property: FromQuery] int PageSize = 10,
        [property: FromQuery] string? OrderBy = null,
        [property: FromQuery] OrderDirectionEnum OrderDirection = OrderDirectionEnum.Ascending);
}

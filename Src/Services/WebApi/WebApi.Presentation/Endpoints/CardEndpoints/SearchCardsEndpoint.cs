using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using CQRS.Core.Contracts;
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
        IQueryManager<SearchCardsQuery, PageableResponse<SearchCardsResponse>> handler,
        CancellationToken cancellationToken,
        int pageNumber = 1,
        int pageSize = 10,
        string? orderBy = null,
        OrderDirectionEnum orderDirection = OrderDirectionEnum.Ascending)
    {
        SearchCardsQuery query = new()
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            OrderBy = orderBy,
            OrderDirection = orderDirection,
        };

        Result<PageableResponse<SearchCardsResponse>> result = await handler.Handle(query, cancellationToken);

        return result.ToMinimalApiResult();
    }
}

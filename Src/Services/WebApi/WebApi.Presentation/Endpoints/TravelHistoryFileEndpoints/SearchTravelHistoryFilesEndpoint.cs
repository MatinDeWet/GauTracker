using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using CQRS.Core.Contracts;
using Microsoft.AspNetCore.Mvc;
using Pagination.Enums;
using Pagination.Models.Responses;
using WebApi.Application.Common.Models;
using WebApi.Application.Features.TravelHistoryFileFeatures.SearchTravelHistoryFiles;

namespace WebApi.Presentation.Endpoints.TravelHistoryFileEndpoints;

internal static class SearchTravelHistoryFilesEndpoint
{
    public static RouteGroupBuilder MapSearchTravelHistoryFilesEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/", SearchTravelHistoryFiles)
            .WithName("SearchTravelHistoryFiles")
            .WithSummary("Returns a paginated list of a card's travel-history files.");

        return group;
    }

    private static async Task<Microsoft.AspNetCore.Http.IResult> SearchTravelHistoryFiles(
        [FromRoute] long cardId,
        [AsParameters] SearchTravelHistoryFilesRequest request,
        [FromServices] IQueryManager<SearchTravelHistoryFilesQuery, PageableResponse<SearchTravelHistoryFilesResponse>> handler,
        CancellationToken cancellationToken)
    {
        DateRange? uploadedBetween = request.DateUploadedFrom is null && request.DateUploadedTo is null
            ? null
            : new DateRange(request.DateUploadedFrom, request.DateUploadedTo);

        SearchTravelHistoryFilesQuery query = new()
        {
            CardId = cardId,
            SearchTerm = request.SearchTerm,
            UploadedBetween = uploadedBetween,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            OrderBy = request.OrderBy,
            OrderDirection = request.OrderDirection,
        };

        Result<PageableResponse<SearchTravelHistoryFilesResponse>> result = await handler.Handle(query, cancellationToken);

        return result.ToMinimalApiResult();
    }

    /// <summary>Query-string parameters for <see cref="SearchTravelHistoryFiles"/>; the card id binds from the route.</summary>
    private readonly record struct SearchTravelHistoryFilesRequest(
        [property: FromQuery] string? SearchTerm = null,
        [property: FromQuery] DateTimeOffset? DateUploadedFrom = null,
        [property: FromQuery] DateTimeOffset? DateUploadedTo = null,
        [property: FromQuery] int PageNumber = 1,
        [property: FromQuery] int PageSize = 10,
        [property: FromQuery] string? OrderBy = null,
        [property: FromQuery] OrderDirectionEnum OrderDirection = OrderDirectionEnum.Ascending);
}

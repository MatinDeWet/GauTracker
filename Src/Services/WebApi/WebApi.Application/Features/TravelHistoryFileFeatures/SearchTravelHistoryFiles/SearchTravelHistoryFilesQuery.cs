using CQRS.Core.Contracts;
using Pagination.Models.Requests;
using Pagination.Models.Responses;
using Searchable.PostgreSQL.Contracts;
using WebApi.Application.Common.Models;

namespace WebApi.Application.Features.TravelHistoryFileFeatures.SearchTravelHistoryFiles;

public sealed class SearchTravelHistoryFilesQuery
    : PageableRequest, IQuery<PageableResponse<SearchTravelHistoryFilesResponse>>, ISearchableRequest
{
    public long CardId { get; init; }

    /// <summary>Full-text search term matched against the file name and display name.</summary>
    public string? SearchTerm { get; init; }

    /// <summary>Optional inclusive range filter on the upload date.</summary>
    public DateRange? UploadedBetween { get; init; }
}

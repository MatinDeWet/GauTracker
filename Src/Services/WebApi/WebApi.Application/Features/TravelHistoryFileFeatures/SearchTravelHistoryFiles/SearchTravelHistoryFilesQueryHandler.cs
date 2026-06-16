using Ardalis.Result;
using CQRS.Core.Contracts;
using Pagination;
using Pagination.Models.Responses;
using Searchable.PostgreSQL;
using Searchable.PostgreSQL.Enums;
using WebApi.Application.Repositories.QueryRepos.SecuredRepos;
using WebApi.Domain.Entities;

namespace WebApi.Application.Features.TravelHistoryFileFeatures.SearchTravelHistoryFiles;

internal sealed class SearchTravelHistoryFilesQueryHandler(ISecuredQueryRepo queryRepo)
    : IQueryManager<SearchTravelHistoryFilesQuery, PageableResponse<SearchTravelHistoryFilesResponse>>
{
    public async Task<Result<PageableResponse<SearchTravelHistoryFilesResponse>>> Handle(SearchTravelHistoryFilesQuery request, CancellationToken cancellationToken)
    {
        IQueryable<TravelHistoryFile> query = queryRepo.TravelHistoryFiles
            .Where(x => x.CardId == request.CardId);

        if (request.UploadedBetween?.From is { } from)
        {
            query = query.Where(x => x.DateUploaded >= from);
        }

        if (request.UploadedBetween?.To is { } to)
        {
            query = query.Where(x => x.DateUploaded <= to);
        }

        query = query.ILikeSearch(
            request,
            [x => x.FileName, x => x.DisplayName!],
            ILikeMatchModeEnum.Contains,
            useOrLogic: true);

        PageableResponse<SearchTravelHistoryFilesResponse> response = await query
            .Select(x => new SearchTravelHistoryFilesResponse
            {
                Id = x.Id,
                CardId = x.CardId,
                FileName = x.FileName,
                DisplayName = x.DisplayName,
                SizeInBytes = x.SizeInBytes,
                Status = x.Status,
                DateUploaded = x.DateUploaded,
                DateCreated = x.DateCreated,
            })
            .ToPageableListAsync(x => x.Id, request, cancellationToken);

        return response;
    }
}

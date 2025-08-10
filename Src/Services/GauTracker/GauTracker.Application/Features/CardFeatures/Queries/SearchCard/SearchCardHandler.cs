using Ardalis.Result;
using CQRS.Core.Contracts;
using GauTracker.Application.Repositories.Query;
using Pagination;
using Pagination.Enums;
using Pagination.Models.Responses;

namespace GauTracker.Application.Features.CardFeatures.Queries.SearchCard;
internal sealed class SearchCardHandler(ICardQueryRepository repo) : IQueryManager<SearchCardRequest, PageableResponse<SearchCardResponse>>
{
    public async Task<Result<PageableResponse<SearchCardResponse>>> Handle(SearchCardRequest query, CancellationToken cancellationToken)
    {
        PageableResponse<SearchCardResponse> response = await repo.Cards
            .Select(x => new SearchCardResponse
            {
                Id = x.Id,
                Alias = x.Alias,
                Number = x.Number,
                CardType = x.CardType,
                ExpiryDate = x.ExpiryDate
            })
            .ToPageableListAsync(x => x.Alias, OrderDirectionEnum.Ascending, query, cancellationToken);

        return response;
    }
}

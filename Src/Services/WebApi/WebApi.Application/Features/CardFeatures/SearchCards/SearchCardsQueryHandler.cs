using Ardalis.Result;
using CQRS.Core.Contracts;
using Pagination;
using Pagination.Models.Responses;
using WebApi.Application.Repositories.QueryRepos.SecuredRepos;

namespace WebApi.Application.Features.CardFeatures.SearchCards;

internal sealed class SearchCardsQueryHandler(ISecuredQueryRepo queryRepo)
    : IQueryManager<SearchCardsQuery, PageableResponse<SearchCardsResponse>>
{
    public async Task<Result<PageableResponse<SearchCardsResponse>>> Handle(SearchCardsQuery request, CancellationToken cancellationToken)
    {
        PageableResponse<SearchCardsResponse> response = await queryRepo.Cards
            .Select(x => new SearchCardsResponse
            {
                Id = x.Id,
                Name = x.Name,
                Number = x.Number,
                ExpiryDate = x.ExpiryDate,
                DateCreated = x.DateCreated,
            })
            .ToPageableListAsync(x => x.Id, request, cancellationToken);

        return response;
    }
}

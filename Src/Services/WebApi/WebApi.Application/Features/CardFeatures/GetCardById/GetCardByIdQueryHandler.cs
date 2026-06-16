using Ardalis.Result;
using CQRS.Core.Contracts;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.Repositories.QueryRepos.SecuredRepos;

namespace WebApi.Application.Features.CardFeatures.GetCardById;

internal sealed class GetCardByIdQueryHandler(ISecuredQueryRepo queryRepo)
    : IQueryManager<GetCardByIdQuery, GetCardByIdResponse>
{
    public async Task<Result<GetCardByIdResponse>> Handle(GetCardByIdQuery request, CancellationToken cancellationToken)
    {
        GetCardByIdResponse? card = await queryRepo.Cards
            .Where(x => x.Id == request.Id)
            .Select(x => new GetCardByIdResponse(x.Id, x.Name, x.Number, x.ExpiryDate, x.DateCreated))
            .FirstOrDefaultAsync(cancellationToken);

        if (card is null)
        {
            return Result.NotFound();
        }

        return card;
    }
}

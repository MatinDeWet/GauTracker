using Ardalis.Result;
using CQRS.Core.Contracts;
using GauTracker.Application.Repositories.Query;
using Microsoft.EntityFrameworkCore;

namespace GauTracker.Application.Features.CardFeatures.Queries.GetCardById;
internal sealed class GetCardByIdHandler(ICardQueryRepository repo) : IQueryManager<GetCardByIdRequest, GetCardByIdResponse>
{
    public async Task<Result<GetCardByIdResponse>> Handle(GetCardByIdRequest query, CancellationToken cancellationToken)
    {
        GetCardByIdResponse? card = await repo.Cards
            .Where(x => x.Id == query.Id)
            .Select(x => new GetCardByIdResponse
            {
                Id = x.Id,
                UserId = x.UserId,
                Alias = x.Alias,
                Number = x.Number,
                CardType = x.CardType,
                ExpiryDate = x.ExpiryDate,
                DateCreated = x.DateCreated
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (card is null)
        {
            return Result.NotFound();
        }

        return card;
    }
}

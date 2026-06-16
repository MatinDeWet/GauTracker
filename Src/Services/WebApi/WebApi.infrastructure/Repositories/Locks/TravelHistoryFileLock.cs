using Microsoft.EntityFrameworkCore;
using Repository.Enums;
using Repository.Lock;
using WebApi.Domain.Entities;
using WebApi.infrastructure.Data.Contexts;

namespace WebApi.infrastructure.Repositories.Locks;

/// <summary>
/// Row-level protection for <see cref="TravelHistoryFile"/>: ownership is transitive through the
/// owning <see cref="Card"/>, so a user may only see and modify files attached to their own cards.
/// </summary>
internal sealed class TravelHistoryFileLock(GauContext context) : Lock<TravelHistoryFile>
{
    public override IQueryable<TravelHistoryFile> Secured(long userId)
    {
        return from file in context.Set<TravelHistoryFile>()
               where file.Card.UserId == userId
               select file;
    }

    public override async Task<bool> HasAccess(TravelHistoryFile obj, long userId, RepositoryOperationEnum operation, CancellationToken cancellationToken)
    {
        IQueryable<Card> ownedCard = from card in context.Set<Card>()
                                     where card.Id == obj.CardId && card.UserId == userId
                                     select card;

        return await ownedCard.AnyAsync(cancellationToken);
    }
}

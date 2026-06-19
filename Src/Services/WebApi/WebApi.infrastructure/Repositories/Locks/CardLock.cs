using Repository.Enums;
using Repository.Lock;
using Shared.Domain.Entities;
using Shared.Persistence.Data.Contexts;

namespace WebApi.infrastructure.Repositories.Locks;

/// <summary>
/// Row-level protection for <see cref="Card"/>: a user may only see and modify cards they own.
/// </summary>
internal sealed class CardLock(GauContext context) : Lock<Card>
{
    public override IQueryable<Card> Secured(long userId)
    {
        return from card in context.Set<Card>()
               where card.UserId == userId
               select card;
    }

    public override Task<bool> HasAccess(Card obj, long userId, RepositoryOperationEnum operation, CancellationToken cancellationToken)
    {
        return Task.FromResult(obj.UserId == userId);
    }
}

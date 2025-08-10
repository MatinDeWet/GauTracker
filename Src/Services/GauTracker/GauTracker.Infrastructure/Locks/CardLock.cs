using Domain.Core.Entities;
using GauTracker.Infrastructure.Data.Contexts;
using Repository.Core.Enums;
using Repository.Core.Lock;

namespace GauTracker.Infrastructure.Locks;
internal sealed class CardLock(GauTrackerContext context) : Lock<Card>
{
    public override async Task<bool> HasAccess(Card obj, Guid identityId, RepositoryOperationEnum operation, CancellationToken cancellationToken)
    {
        Guid userId = obj.UserId;

        return await Task.FromResult(userId == identityId);
    }

    public override IQueryable<Card> Secured(Guid identityId)
    {
        IQueryable<Card> query =
            from c in context.Set<Card>()
            where c.UserId == identityId
            select c;

        return query;
    }
}

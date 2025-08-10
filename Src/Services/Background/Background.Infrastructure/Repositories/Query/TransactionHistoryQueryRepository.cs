using Background.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Repository.Core.Implementation;

namespace Background.Infrastructure.Repositories.Query;
internal sealed class TransactionHistoryQueryRepository : QueryRepo<GauTrackerContext>, ITransactionHistoryQueryRepository
{
    public TransactionHistoryQueryRepository(GauTrackerContext context)
        : base(context)
    {
    }

    public IQueryable<TransactionHistory> TransactionHistory => GetQueryable<TransactionHistory>();

    public async Task<HashSet<int>> GetExistingSequenceNumbersAsync(Guid cardId, CancellationToken cancellationToken = default)
    {
        HashSet<int> sequenceNumbers = await TransactionHistory
            .Where(th => th.CardId == cardId)
            .Select(th => th.SequenceNumber)
            .ToHashSetAsync(cancellationToken);

        return sequenceNumbers;
    }
}

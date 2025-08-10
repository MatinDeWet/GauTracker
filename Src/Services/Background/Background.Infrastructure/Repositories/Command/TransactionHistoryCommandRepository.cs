using Background.Infrastructure.Data.Contexts;
using Repository.Core.Implementation;

namespace Background.Infrastructure.Repositories.Command;
internal sealed class TransactionHistoryCommandRepository : CommandRepo<GauTrackerContext>, ITransactionHistoryCommandRepository
{
    public TransactionHistoryCommandRepository(GauTrackerContext context)
        : base(context)
    {
    }
}

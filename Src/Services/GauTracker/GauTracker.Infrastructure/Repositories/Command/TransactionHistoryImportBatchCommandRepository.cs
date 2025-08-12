using GauTracker.Application.Repositories.Command;
using GauTracker.Infrastructure.Data.Contexts;
using Identification.Base;
using Repository.Core.Contracts;
using Repository.Core.Implementation;

namespace GauTracker.Infrastructure.Repositories.Command;
internal sealed class TransactionHistoryImportBatchCommandRepository : SecureCommandRepo<GauTrackerContext>, ITransactionHistoryImportBatchCommandRepository
{
    public TransactionHistoryImportBatchCommandRepository(GauTrackerContext context, IIdentityInfo info, IEnumerable<IProtected> protection)
        : base(context, info, protection)
    {
    }
}

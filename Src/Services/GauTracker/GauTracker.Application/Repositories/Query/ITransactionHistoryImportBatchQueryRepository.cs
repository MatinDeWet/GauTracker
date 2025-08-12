using Domain.Core.Entities;
using Repository.Base;

namespace GauTracker.Application.Repositories.Query;
public interface ITransactionHistoryImportBatchQueryRepository : ISecureQueryRepo
{
    IQueryable<TransactionHistoryImportBatch> TransactionHistoryImportBatchs { get; }
}

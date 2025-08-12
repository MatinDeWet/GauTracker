using CQRS.Event.Base.Contracts;
using Domain.Core.Entities;

namespace Domain.Core.Events;
public record TransactionHistoryImportBatchCreatedEvent(TransactionHistoryImportBatch transactionHistoryImportBatch) : IDomainEvent;

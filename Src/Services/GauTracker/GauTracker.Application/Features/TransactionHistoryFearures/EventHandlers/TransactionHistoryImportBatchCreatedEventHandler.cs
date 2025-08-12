using CQRS.Core.Contracts;
using Domain.Core.Events;
using Messaging.Core.Abstractions;
using Messaging.Event.Events;

namespace GauTracker.Application.Features.TransactionHistoryFearures.EventHandlers;
internal sealed class TransactionHistoryImportBatchCreatedEventHandler(IIntegrationEventPublisher eventPublisher) : IDomainEventManager<TransactionHistoryImportBatchCreatedEvent>
{
    public async Task Handle(TransactionHistoryImportBatchCreatedEvent domainEvent, CancellationToken cancellationToken)
    {
        var @event = new TransactionHistoryImportBatchCreatedExternalEvent(domainEvent.transactionHistoryImportBatch.Id);

        await eventPublisher.PublishAsync(@event, cancellationToken);
    }
}

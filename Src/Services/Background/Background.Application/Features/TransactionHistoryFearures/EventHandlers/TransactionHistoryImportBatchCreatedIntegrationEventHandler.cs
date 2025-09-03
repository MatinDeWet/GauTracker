using MassTransit;
using Messaging.Event.Events;

namespace Background.Application.Features.TransactionHistoryFearures.EventHandlers;
internal sealed class TransactionHistoryImportBatchCreatedIntegrationEventHandler : IConsumer<TransactionHistoryImportBatchCreatedExternalEvent>
{
    public Task Consume(ConsumeContext<TransactionHistoryImportBatchCreatedExternalEvent> context)
    {
        throw new NotImplementedException();
    }
}

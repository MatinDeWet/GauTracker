using MassTransit;
using Messaging.Core.Abstractions;
using Messaging.Event.Common.Contracts;
using Microsoft.Extensions.Logging;

namespace Messaging.Core.MassTransit;

public sealed class IntegrationEventConsumerAdapter<TEvent> : IConsumer<TEvent>
    where TEvent : class, IIntegrationEvent
{
    private readonly IEnumerable<IIntegrationEventHandler<TEvent>> _handlers;
    private readonly ILogger<IntegrationEventConsumerAdapter<TEvent>> _logger;

    public IntegrationEventConsumerAdapter(
        IEnumerable<IIntegrationEventHandler<TEvent>> handlers,
        ILogger<IntegrationEventConsumerAdapter<TEvent>> logger)
    {
        _handlers = handlers;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<TEvent> context)
    {
        if (_handlers is null || !_handlers.Any())
        {
            _logger.LogWarning("No handlers registered for integration event type {EventType}. MessageId={MessageId} CorrelationId={CorrelationId}",
                typeof(TEvent).Name,
                context.MessageId,
                context.CorrelationId);
            return;
        }

        foreach (IIntegrationEventHandler<TEvent> handler in _handlers)
        {
            await handler.HandleAsync(context.Message, context.CancellationToken).ConfigureAwait(false);
        }
    }
}

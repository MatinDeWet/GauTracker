using Messaging.Event.Common.Contracts;

namespace Messaging.Core.Abstractions;

public interface IIntegrationEventPublisher
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class, IIntegrationEvent;
}

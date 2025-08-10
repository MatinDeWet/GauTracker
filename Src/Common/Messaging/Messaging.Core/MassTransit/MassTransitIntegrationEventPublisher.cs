using MassTransit;
using Messaging.Core.Abstractions;
using Messaging.Event.Common.Contracts;

namespace Messaging.Core.MassTransit;

internal sealed class MassTransitIntegrationEventPublisher : IIntegrationEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public MassTransitIntegrationEventPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class, IIntegrationEvent
    {
        return _publishEndpoint.Publish(@event, cancellationToken);
    }
}

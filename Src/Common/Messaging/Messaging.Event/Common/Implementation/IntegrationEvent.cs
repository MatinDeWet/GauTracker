using Messaging.Event.Common.Contracts;

namespace Messaging.Event.Common.Implementation;

public record IntegrationEvent : IIntegrationEvent
{
    public Guid Id => Guid.NewGuid();
    public DateTime OccurredOn => DateTime.UtcNow;
    public string EventType => GetType().AssemblyQualifiedName;
}

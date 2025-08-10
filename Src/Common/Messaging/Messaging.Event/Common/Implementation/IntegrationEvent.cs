using Messaging.Event.Common.Contracts;

namespace Messaging.Event.Common.Implementation;

public record IntegrationEvent : IIntegrationEvent
{
    // Capture once per instance to keep stable values
    public Guid Id { get; init; }

    public DateTime OccurredOn { get; init; }

    public string EventType { get; init; }

    public IntegrationEvent()
    {
        Id = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        EventType = GetType().AssemblyQualifiedName ?? GetType().FullName ?? GetType().Name;
    }
}

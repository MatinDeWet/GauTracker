namespace Messaging.Event.Common.Contracts;
public interface IIntegrationEvent
{
    Guid Id { get; }

    DateTime OccurredOn { get; }

    string EventType { get; }
}

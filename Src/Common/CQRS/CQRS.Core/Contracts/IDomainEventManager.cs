using CQRS.Event.Base.Contracts;

namespace CQRS.Core.Contracts;
public interface IDomainEventManager<in T> where T : IDomainEvent
{
    Task Handle(T domainEvent, CancellationToken cancellationToken);
}

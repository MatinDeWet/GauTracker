using CQRS.Event.Base.Contracts;

namespace CQRS.Event.Core.Contracts;

public interface IDomainEventsDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}

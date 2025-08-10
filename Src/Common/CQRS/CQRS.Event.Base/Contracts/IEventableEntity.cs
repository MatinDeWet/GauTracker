using Domain.Support.Contracts;

namespace CQRS.Event.Base.Contracts;

public interface IEventableEntity<T> : IEventableEntity, IEntity<T>
{
}

public interface IEventableEntity : IEntity
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }

    void ClearDomainEvents();

    void AddDomainEvent(IDomainEvent domainEvent);
}

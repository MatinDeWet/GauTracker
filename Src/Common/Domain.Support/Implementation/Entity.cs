using Domain.Support.Contracts;

namespace Domain.Support.Implementation;
public abstract class Entity<T> : Entity, IEntity<T>
{
    public T Id { get; protected set; } = default!;
}

public abstract class Entity : IEntity
{
    public DateTimeOffset DateCreated { get; private set; } = DateTimeOffset.UtcNow;
}

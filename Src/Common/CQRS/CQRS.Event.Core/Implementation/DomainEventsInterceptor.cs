using CQRS.Event.Base.Contracts;
using CQRS.Event.Core.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CQRS.Event.Core.Implementation;
public class DomainEventsInterceptor(IDomainEventsDispatcher domainEventsDispatcher) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        InterceptionResult<int> response = base.SavingChanges(eventData, result);

        DispatchDomainEvents(eventData.Context).GetAwaiter().GetResult();

        return response;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        InterceptionResult<int> response = await base.SavingChangesAsync(eventData, result, cancellationToken);

        await DispatchDomainEvents(eventData.Context);

        return response;
    }

    private async Task DispatchDomainEvents(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var domainEvents = context.ChangeTracker
            .Entries<IEventableEntity>()
            .Where(a => a.Entity.DomainEvents.Any())
            .Select(a => a.Entity)
            .SelectMany(entity =>
            {
                IReadOnlyList<IDomainEvent> domainEvents = [.. entity.DomainEvents];
                entity.ClearDomainEvents();
                return domainEvents;
            })
            .ToList();

        await domainEventsDispatcher.DispatchAsync(domainEvents);
    }
}

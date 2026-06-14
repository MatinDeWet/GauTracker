using System.Threading;
using System.Threading.Tasks;
using CQRS.Core.Contracts;
using CQRS.Domain.Contracts;

namespace CQRS.UnitTests.TestDoubles;

public sealed record TestDomainEvent(string Message) : IDomainEvent;

/// <summary>
/// Singleton bridge used by tests to observe that a domain event reached its handler.
/// </summary>
public sealed class HandlerSignal
{
    public TaskCompletionSource<TestDomainEvent> Received { get; } =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
}

/// <summary>
/// Discovered by <c>AddCQRSSupport</c>'s assembly scan and invoked by the background processor.
/// </summary>
internal sealed class TestDomainEventHandler(HandlerSignal signal) : IDomainEventManager<TestDomainEvent>
{
    public Task Handle(TestDomainEvent request, CancellationToken cancellationToken)
    {
        signal.Received.TrySetResult(request);
        return Task.CompletedTask;
    }
}

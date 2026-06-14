using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Core;
using CQRS.Core.Services;
using CQRS.UnitTests.TestDoubles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shouldly;
using Xunit;

namespace CQRS.UnitTests.Tests;

public class BackgroundDomainEventProcessorTests
{
    [Fact]
    public async Task ExecuteAsync_DispatchesEnqueuedEvent_ToDiscoveredHandler()
    {
        CancellationToken ct = TestContext.Current.CancellationToken;
        HandlerSignal signal = new();
        ServiceCollection services = new();
        services.AddLogging();
        services.AddSingleton(signal);
        services.AddCQRSSupport(typeof(BackgroundDomainEventProcessorTests));

        await using ServiceProvider provider = services.BuildServiceProvider();

        BackgroundDomainEventProcessor processor = provider.GetServices<IHostedService>()
            .OfType<BackgroundDomainEventProcessor>()
            .Single();
        IBackgroundDomainEventQueue queue = provider.GetRequiredService<IBackgroundDomainEventQueue>();

        await processor.StartAsync(ct);
        try
        {
            TestDomainEvent published = new("hello");
            await queue.EnqueueAsync(published, ct);

            TestDomainEvent handled = await signal.Received.Task.WaitAsync(TimeSpan.FromSeconds(5), ct);

            handled.ShouldBe(published);
        }
        finally
        {
            await processor.StopAsync(CancellationToken.None);
        }
    }
}

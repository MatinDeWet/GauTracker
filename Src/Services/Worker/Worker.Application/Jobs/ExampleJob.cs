using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repository.Contracts;
using Shared.Domain.Entities;
using Worker.Application.Logging;

namespace Worker.Application.Jobs;

internal sealed class ExampleJob(IQueryRepo queryRepo, ILogger<ExampleJob> logger) : IExampleJob
{
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        int cardCount = await queryRepo.GetQueryable<Card>().CountAsync(cancellationToken);

        logger.ExampleJobRan(cardCount);
    }
}

using Ardalis.Result;
using Background.Application.Repositories.Command;
using Background.Application.Repositories.Query;
using CQRS.Core.Contracts;

namespace Background.Application.Features.CardFeatures.DeleteExpiredCard;
internal sealed class DeleteExpiredCardHandler(ICardQueryRepository queryRepo, ICardCommandRepository commandRepo) : ICommandManager<DeleteExpiredCardRequest>
{
    public async Task<Result> Handle(DeleteExpiredCardRequest command, CancellationToken cancellationToken)
    {
        var cutoffDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30));

        List<Card> expiredCards = await queryRepo.Cards
            .Where(card => card.ExpiryDate < cutoffDate)
            .ToListAsync(cancellationToken);

        if (expiredCards.Any())
        {
            foreach (Card card in expiredCards)
            {
                await commandRepo.DeleteAsync(card, cancellationToken);
            }

            await commandRepo.SaveAsync(cancellationToken);
        }

        return Result.Success();
    }
}

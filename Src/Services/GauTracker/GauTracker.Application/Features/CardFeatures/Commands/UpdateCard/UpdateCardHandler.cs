using Ardalis.Result;
using CQRS.Core.Contracts;
using Domain.Core.Entities;
using GauTracker.Application.Repositories.Command;
using GauTracker.Application.Repositories.Query;
using Microsoft.EntityFrameworkCore;

namespace GauTracker.Application.Features.CardFeatures.Commands.UpdateCard;
internal sealed class UpdateCardHandler(ICardQueryRepository queryRepo, ICardCommandRepository commandRepo) : ICommandManager<UpdateCardRequest>
{
    public async Task<Result> Handle(UpdateCardRequest command, CancellationToken cancellationToken)
    {
        Card? card = await queryRepo.Cards
            .Where(x => x.Id == command.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (card is null)
        {
            return Result.NotFound();
        }

        card.Update(
            command.Alias,
            command.Number,
            command.CardType,
            command.ExpiryDate
        );

        await commandRepo.UpdateAsync(card, true, cancellationToken);

        return Result.Success();
    }
}

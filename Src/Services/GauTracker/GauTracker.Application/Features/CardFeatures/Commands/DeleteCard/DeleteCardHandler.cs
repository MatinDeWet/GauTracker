using Ardalis.Result;
using CQRS.Core.Contracts;
using Domain.Core.Entities;
using GauTracker.Application.Repositories.Command;
using GauTracker.Application.Repositories.Query;
using Microsoft.EntityFrameworkCore;

namespace GauTracker.Application.Features.CardFeatures.Commands.DeleteCard;
internal sealed class DeleteCardHandler(ICardQueryRepository queryRepo, ICardCommandRepository commandRepo) : ICommandManager<DeleteCardRequest>
{
    public async Task<Result> Handle(DeleteCardRequest command, CancellationToken cancellationToken)
    {
        Card? card = await queryRepo.Cards
             .Where(x => x.Id == command.Id)
             .FirstOrDefaultAsync(cancellationToken);

        if (card is null)
        {
            return Result.NotFound();
        }

        await commandRepo.DeleteAsync(card, true, cancellationToken);

        return Result.Success();
    }
}

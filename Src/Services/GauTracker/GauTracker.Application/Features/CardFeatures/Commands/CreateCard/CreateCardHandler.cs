using Ardalis.Result;
using CQRS.Core.Contracts;
using Domain.Core.Entities;
using GauTracker.Application.Repositories.Command;
using GauTracker.Application.Repositories.Query;
using Identification.Base;

namespace GauTracker.Application.Features.CardFeatures.Commands.CreateCard;
internal sealed class CreateCardHandler(ICardCommandRepository commandRepo, ICardQueryRepository queryRepo, IIdentityInfo identityInfo) : ICommandManager<CreateCardRequest, Guid>
{
    public async Task<Result<Guid>> Handle(CreateCardRequest command, CancellationToken cancellationToken)
    {
        if (await queryRepo.NumberExists(command.Number, cancellationToken))
        {
            return Result.Conflict("Card with this number already exists.");
        }

        var card = Card.Create(
            identityInfo.GetIdentityId(),
            command.Alias,
            command.Number,
            command.CardType,
            command.ExpiryDate
        );

        await commandRepo.InsertAsync(card, true, cancellationToken);

        return card.Id;
    }
}

using Ardalis.Result;
using CQRS.Core.Contracts;
using Domain.Core.Entities;
using GauTracker.Application.Repositories.Command;
using Identification.Base;

namespace GauTracker.Application.Features.CardFeatures.Commands.CreateCard;
internal sealed class CreateCardHandler(ICardCommandRepository repo, IIdentityInfo identityInfo) : ICommandManager<CreateCardRequest, Guid>
{
    public async Task<Result<Guid>> Handle(CreateCardRequest command, CancellationToken cancellationToken)
    {
        var card = Card.Create(
            identityInfo.GetIdentityId(),
            command.Alias,
            command.Number,
            command.CardType,
            command.ExpiryDate
        );

        await repo.InsertAsync(card, true, cancellationToken);

        return card.Id;
    }
}

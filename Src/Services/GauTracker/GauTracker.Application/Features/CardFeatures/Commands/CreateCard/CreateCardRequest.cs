using CQRS.Base;
using Domain.Core.Enums;

namespace GauTracker.Application.Features.CardFeatures.Commands.CreateCard;
public sealed record CreateCardRequest : ICommand<Guid>
{
    public string Alias { get; set; }

    public string Number { get; set; }

    public CardTypeEnum CardType { get; set; }

    public DateOnly ExpiryDate { get; set; }
}

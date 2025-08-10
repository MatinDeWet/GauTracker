using CQRS.Base;

namespace GauTracker.Application.Features.CardFeatures.Commands.DeleteCard;
public sealed record DeleteCardRequest : ICommand
{
    public Guid Id { get; set; }
}

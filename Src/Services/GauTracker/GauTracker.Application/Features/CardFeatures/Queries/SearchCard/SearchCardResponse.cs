using Domain.Core.Enums;

namespace GauTracker.Application.Features.CardFeatures.Queries.SearchCard;
public sealed record SearchCardResponse
{
    public Guid Id { get; set; }

    public string? Alias { get; set; }

    public string Number { get; set; }

    public CardTypeEnum CardType { get; set; }

    public DateOnly ExpiryDate { get; set; }
}

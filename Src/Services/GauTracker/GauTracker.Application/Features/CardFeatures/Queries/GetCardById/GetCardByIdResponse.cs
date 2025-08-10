using Domain.Core.Enums;

namespace GauTracker.Application.Features.CardFeatures.Queries.GetCardById;
public sealed record GetCardByIdResponse
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string? Alias { get; set; }

    public string Number { get; set; }

    public CardTypeEnum CardType { get; set; }

    public DateOnly ExpiryDate { get; set; }

    public DateTimeOffset DateCreated { get; set; }
}

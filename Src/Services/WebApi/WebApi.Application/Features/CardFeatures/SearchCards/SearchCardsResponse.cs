namespace WebApi.Application.Features.CardFeatures.SearchCards;

public sealed record SearchCardsResponse
{
    public long Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Number { get; init; } = string.Empty;

    public DateOnly ExpiryDate { get; init; }

    public DateTimeOffset DateCreated { get; init; }
}

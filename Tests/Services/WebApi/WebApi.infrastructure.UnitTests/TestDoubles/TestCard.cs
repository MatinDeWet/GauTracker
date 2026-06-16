using WebApi.Domain.Entities;

namespace WebApi.infrastructure.UnitTests.TestDoubles;

/// <summary>
/// Builds <see cref="Card"/> instances with a specific identifier and owner. <see cref="Card.Id"/>
/// has a protected setter (it is database-generated), so it is assigned via reflection for tests
/// that need to exercise identity-based filtering.
/// </summary>
internal static class TestCard
{
    public static Card OwnedBy(long userId, long id = 0)
    {
        var card = Card.Create(userId, $"card-{id}", $"number-{id}", new DateOnly(2030, 1, 1));

        if (id != 0)
        {
            typeof(Card).GetProperty(nameof(Card.Id))!.SetValue(card, id);
        }

        return card;
    }
}

using WebApi.Domain.Entities;

namespace WebApi.infrastructure.UnitTests.TestDoubles;

/// <summary>
/// Builds <see cref="TravelHistoryFile"/> instances attached to a given card/owner. The
/// <see cref="TravelHistoryFile.Card"/> navigation is populated so the lock's transitive
/// <c>Card.UserId</c> filter can be exercised in-memory; database-generated identifiers are
/// assigned via reflection.
/// </summary>
internal static class TestTravelHistoryFile
{
    public static TravelHistoryFile ForCard(long cardId, long userId, long id = 0)
    {
        var file = TravelHistoryFile.Create(
            cardId,
            "history.csv",
            "text/csv",
            "transactionfile-2026",
            $"{cardId}/file-{id}.csv");

        Card card = TestCard.OwnedBy(userId, cardId);
        typeof(TravelHistoryFile).GetProperty(nameof(TravelHistoryFile.Card))!.SetValue(file, card);

        if (id != 0)
        {
            typeof(TravelHistoryFile).GetProperty(nameof(TravelHistoryFile.Id))!.SetValue(file, id);
        }

        return file;
    }
}

using Ardalis.GuardClauses;
using Domain.Extensions;
using Domain.Implementation;

namespace Shared.Domain.Entities;

public class Card : Entity<long>
{
    public long UserId { get; private set; }

    public virtual User User { get; private set; } = null!;

    public virtual ICollection<TravelHistoryFile> TravelHistoryFiles { get; private set; } = [];

    public string Name { get; private set; }

    public string Number { get; private set; }

    public DateOnly ExpiryDate { get; private set; }

    public static Card Create(long userId, string name, string number, DateOnly expiryDate)
    {
        return new Card
        {
            UserId = userId,
            Name = ValidName(name),
            Number = ValidNumber(number),
            ExpiryDate = expiryDate
        };
    }

    public void Update(string name, string number, DateOnly expiryDate)
    {
        Name = ValidName(name);
        Number = ValidNumber(number);
        ExpiryDate = expiryDate;
    }

    private static string ValidName(string name)
    {
        return Guard.Against.ValidString(name, nameof(name), maxLength: 64);
    }

    private static string ValidNumber(string number)
    {
        return Guard.Against.ValidString(number, nameof(number), maxLength: 64);
    }

}

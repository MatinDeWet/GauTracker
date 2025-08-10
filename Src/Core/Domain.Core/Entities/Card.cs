using Domain.Core.Enums;
using Domain.Support.Implementation;

namespace Domain.Core.Entities;
public class Card : Entity<Guid>
{
    public Guid UserId { get; private set; }
    public virtual User User { get; private set; }

    public string Alias { get; private set; }

    public string Number { get; private set; }

    public CardTypeEnum CardType { get; private set; }

    public DateOnly ExpiryDate { get; private set; }

    public static Card Create(Guid userId, string alias, string number, CardTypeEnum cardType, DateOnly expiryDate)
    {
        return new Card
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            Alias = alias,
            Number = number,
            CardType = cardType,
            ExpiryDate = expiryDate
        };
    }

    public void Update(string alias, string number, CardTypeEnum cardType, DateOnly expiryDate)
    {
        Alias = alias;
        Number = number;
        CardType = cardType;
        ExpiryDate = expiryDate;
    }
}

using Domain.Support.Implementation;

namespace GauTracker.Domain.Entities;
public class UserRefreshToken : Entity<Guid>
{
    public Guid UserID { get; private set; }
    public virtual ApplicationUser User { get; private set; } = null!;

    public string Token { get; private set; }
    public DateTime ExpiryDate { get; private set; }

    public static UserRefreshToken Create(Guid userId, string token, DateTime expirationDate)
    {
        return new UserRefreshToken
        {
            Id = Guid.CreateVersion7(),
            UserID = userId,
            Token = token,
            ExpiryDate = expirationDate,
        };
    }
}

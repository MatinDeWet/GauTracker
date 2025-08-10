using Domain.Support.Implementation;

namespace Domain.Core.Entities;
public class User : Entity<Guid>
{
    public string Email { get; private set; }

    public virtual ICollection<Card> Cards { get; private set; } = [];

    public static User Create(Guid id, string email)
    {
        return new User
        {
            Id = id,
            Email = email
        };
    }
}

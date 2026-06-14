using Domain.Implementation;

namespace WebApi.Domain.Entities;

public class User : Entity<long>
{
    public virtual ICollection<Card> Cards { get; private set; } = [];
}

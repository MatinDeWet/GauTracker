using Domain.Support.Implementation;

namespace Domain.Core.Entities;
public class TransportMode : Entity<int>
{
    public string Name { get; private set; }

    public virtual ICollection<StationTransportMode> StationTransportModes { get; private set; } = [];

    public static TransportMode Create(string name)
    {
        return new TransportMode
        {
            Name = name
        };
    }
}

using Domain.Support.Implementation;
using NetTopologySuite.Geometries;

namespace Domain.Core.Entities;
public class Station : Entity<int>
{
    public string ExternalId { get; private set; }

    public string Name { get; private set; }

    public Point Location { get; set; }

    public DateTimeOffset DateUpdated { get; private set; }

    public virtual ICollection<StationTransportMode> StationTransportModes { get; private set; } = [];

    public static Station Create(string externalId, string name, Point location)
    {
        return new Station
        {
            ExternalId = externalId,
            Name = name,
            Location = location,
            DateUpdated = DateTimeOffset.UtcNow
        };
    }

    public void UpdateName(string name)
    {
        if (!string.IsNullOrWhiteSpace(name) && Name.Equals(name, StringComparison.Ordinal))
        {
            return;
        }

        Name = name;

        DateUpdated = DateTimeOffset.UtcNow;
    }

    public void UpdateGeometry(Point geometry)
    {
        if (Location != null && Location.Equals(geometry))
        {
            return;
        }

        Location = geometry;

        DateUpdated = DateTimeOffset.UtcNow;
    }

    public void AddTransportMode(int transportModeId)
    {
        var stationTransportMode = StationTransportMode.Create(Id, transportModeId);
        StationTransportModes.Add(stationTransportMode);
    }

    public void RemoveTransportMode(int transportModeId)
    {
        StationTransportMode? existingRelation = StationTransportModes
            .FirstOrDefault(stm => stm.TransportModeId == transportModeId);

        if (existingRelation is not null)
        {
            StationTransportModes.Remove(existingRelation);
        }
    }
}

using Domain.Support.Implementation;
using NetTopologySuite.Geometries;

namespace Domain.Core.Entities;
public class Station : Entity<int>
{
    public string ExternalId { get; private set; }

    public string Name { get; private set; }

    public Point Location { get; set; }

    public virtual ICollection<StationTransportMode> StationTransportModes { get; private set; } = [];

    public static Station Create(string externalId, string name, Point location)
    {
        return new Station
        {
            ExternalId = externalId,
            Name = name,
            Location = location
        };
    }

    public void UpdateName(string name)
    {
        Name = name;
    }

    public void UpdateGeometry(Point geometry)
    {
        Location = geometry;
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

    public bool HasTransportMode(int transportModeId)
    {
        return StationTransportModes.Any(stm => stm.TransportModeId == transportModeId);
    }
}

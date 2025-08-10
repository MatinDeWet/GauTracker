namespace Domain.Core.Entities;
public class StationTransportMode
{
    public int StationId { get; private set; }
    public virtual Station Station { get; private set; }

    public int TransportModeId { get; private set; }
    public virtual TransportMode TransportMode { get; private set; } = null!;

    public static StationTransportMode Create(int stationId, int transportModeId)
    {
        return new StationTransportMode
        {
            StationId = stationId,
            TransportModeId = transportModeId
        };
    }
}

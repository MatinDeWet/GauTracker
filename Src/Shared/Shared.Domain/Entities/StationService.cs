namespace Shared.Domain.Entities;

/// <summary>
/// Join entity mapping which <see cref="Service"/>s are available at a given <see cref="Station"/>.
/// </summary>
public class StationService
{
    public int StationId { get; private set; }

    public virtual Station Station { get; private set; } = null!;

    public int ServiceId { get; private set; }

    public virtual Service Service { get; private set; } = null!;

    public static StationService Create(int stationId, int serviceId)
    {
        return new StationService
        {
            StationId = stationId,
            ServiceId = serviceId,
        };
    }
}

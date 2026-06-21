using Shared.Domain.Entities;

namespace Shared.Persistence.Data.Seeding;

internal static class StationServiceSeedData
{
    private const int AirportStationId = 10;

    // Services available everywhere: Train(1).
    private static readonly int[] UniversalServiceIds = [1];

    // Services available at every station except OR Tambo(10): Parking(2), Buses(3).
    private static readonly int[] NonAirportServiceIds = [2, 3];

    public static readonly StationService[] StationServices = Build();

    private static StationService[] Build()
    {
        var rows = new List<StationService>();

        foreach (Station station in StationSeedData.Stations)
        {
            foreach (int serviceId in UniversalServiceIds)
            {
                rows.Add(StationService.Create(station.Id, serviceId));
            }

            if (station.Id == AirportStationId)
            {
                continue;
            }

            foreach (int serviceId in NonAirportServiceIds)
            {
                rows.Add(StationService.Create(station.Id, serviceId));
            }
        }

        return [.. rows];
    }
}

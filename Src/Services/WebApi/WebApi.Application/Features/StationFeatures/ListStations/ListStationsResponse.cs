namespace WebApi.Application.Features.StationFeatures.ListStations;

public sealed record ListStationsResponse
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;
}

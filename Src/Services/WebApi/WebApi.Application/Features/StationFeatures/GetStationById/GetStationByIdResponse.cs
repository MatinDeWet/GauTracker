using Shared.Domain.Enums;

namespace WebApi.Application.Features.StationFeatures.GetStationById;

public sealed record GetStationByIdResponse(
    int Id,
    string Name,
    string Address,
    decimal Latitude,
    decimal Longitude,
    StationType StationType,
    bool IsTerminal);

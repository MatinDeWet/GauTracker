using CQRS.Core.Contracts;

namespace WebApi.Application.Features.StationFeatures.ListStations;

public sealed record ListStationsQuery : IQuery<IReadOnlyList<ListStationsResponse>>;

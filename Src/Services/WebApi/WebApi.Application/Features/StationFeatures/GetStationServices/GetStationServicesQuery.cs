using CQRS.Core.Contracts;

namespace WebApi.Application.Features.StationFeatures.GetStationServices;

public sealed record GetStationServicesQuery(int StationId) : IQuery<IReadOnlyList<GetStationServicesResponse>>;

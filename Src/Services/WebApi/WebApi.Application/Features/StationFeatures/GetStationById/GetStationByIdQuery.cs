using CQRS.Core.Contracts;

namespace WebApi.Application.Features.StationFeatures.GetStationById;

public sealed record GetStationByIdQuery(int Id) : IQuery<GetStationByIdResponse>;

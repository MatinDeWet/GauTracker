using CQRS.Core.Contracts;

namespace WebApi.Application.Features.TravelHistoryFileFeatures.GetTravelHistoryFileById;

public sealed record GetTravelHistoryFileByIdQuery(long Id) : IQuery<GetTravelHistoryFileByIdResponse>;

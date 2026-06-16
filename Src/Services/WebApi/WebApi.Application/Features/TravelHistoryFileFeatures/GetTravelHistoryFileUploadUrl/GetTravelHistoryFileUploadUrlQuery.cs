using CQRS.Core.Contracts;

namespace WebApi.Application.Features.TravelHistoryFileFeatures.GetTravelHistoryFileUploadUrl;

public sealed record GetTravelHistoryFileUploadUrlQuery(long Id) : IQuery<GetTravelHistoryFileUploadUrlResponse>;

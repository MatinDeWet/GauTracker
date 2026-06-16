using CQRS.Core.Contracts;

namespace WebApi.Application.Features.TravelHistoryFileFeatures.GetTravelHistoryFileDownloadUrl;

public sealed record GetTravelHistoryFileDownloadUrlQuery(long Id) : IQuery<GetTravelHistoryFileDownloadUrlResponse>;

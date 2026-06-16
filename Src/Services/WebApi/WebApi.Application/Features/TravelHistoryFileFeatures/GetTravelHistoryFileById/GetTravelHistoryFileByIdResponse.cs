using WebApi.Domain.Enums;

namespace WebApi.Application.Features.TravelHistoryFileFeatures.GetTravelHistoryFileById;

public sealed record GetTravelHistoryFileByIdResponse(
    long Id,
    long CardId,
    string FileName,
    string? DisplayName,
    string ContentType,
    long? SizeInBytes,
    TravelHistoryFileStatus Status,
    DateTimeOffset? DateUploaded,
    DateTimeOffset DateCreated);

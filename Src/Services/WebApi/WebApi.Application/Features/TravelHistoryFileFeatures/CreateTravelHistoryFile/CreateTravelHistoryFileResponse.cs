namespace WebApi.Application.Features.TravelHistoryFileFeatures.CreateTravelHistoryFile;

public sealed record CreateTravelHistoryFileResponse(
    long Id,
    string BlobContainer,
    string BlobKey,
    Uri UploadUrl,
    DateTimeOffset ExpiresAt);

namespace WebApi.Application.Features.TravelHistoryFileFeatures.GetTravelHistoryFileUploadUrl;

public sealed record GetTravelHistoryFileUploadUrlResponse(string BlobKey, Uri UploadUrl, DateTimeOffset ExpiresAt);

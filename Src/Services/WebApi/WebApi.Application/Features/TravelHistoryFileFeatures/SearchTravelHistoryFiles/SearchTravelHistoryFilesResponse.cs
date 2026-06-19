using Shared.Domain.Enums;

namespace WebApi.Application.Features.TravelHistoryFileFeatures.SearchTravelHistoryFiles;

public sealed record SearchTravelHistoryFilesResponse
{
    public long Id { get; init; }

    public long CardId { get; init; }

    public string FileName { get; init; } = string.Empty;

    public string? DisplayName { get; init; }

    public long? SizeInBytes { get; init; }

    public TravelHistoryFileStatus Status { get; init; }

    public DateTimeOffset? DateUploaded { get; init; }

    public DateTimeOffset DateCreated { get; init; }
}

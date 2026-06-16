namespace WebApi.Application.Features.TravelHistoryFileFeatures;

/// <summary>
/// Shared CSV validation/normalization used when accepting travel-history uploads. Only the
/// declared metadata can be checked here — the bytes are uploaded directly to blob storage.
/// </summary>
internal static class CsvFile
{
    public const string DefaultContentType = "text/csv";

    private static readonly string[] AllowedContentTypes =
        ["text/csv", "application/csv", "application/vnd.ms-excel"];

    public static bool HasCsvExtension(string? fileName)
    {
        return !string.IsNullOrWhiteSpace(fileName)
            && fileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase);
    }

    public static string NormalizeContentType(string? contentType)
    {
        return string.IsNullOrWhiteSpace(contentType) ? DefaultContentType : contentType;
    }

    public static bool IsAllowedContentType(string contentType)
    {
        return AllowedContentTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase);
    }
}

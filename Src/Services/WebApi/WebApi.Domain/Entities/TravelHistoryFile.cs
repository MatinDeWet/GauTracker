using Ardalis.GuardClauses;
using Domain.Extensions;
using Domain.Implementation;
using WebApi.Domain.Enums;

namespace WebApi.Domain.Entities;

/// <summary>
/// Metadata for a Gautrain travel-history CSV file. The file bytes live in blob storage; this
/// entity tracks ownership (via the owning <see cref="Card"/>), the blob location, and the upload
/// lifecycle.
/// </summary>
public class TravelHistoryFile : Entity<long>
{
    public long CardId { get; private set; }

    public virtual Card Card { get; private set; } = null!;

    public string FileName { get; private set; }

    public string? DisplayName { get; private set; }

    public string ContentType { get; private set; }

    public string BlobContainer { get; private set; }

    public string BlobKey { get; private set; }

    public long? SizeInBytes { get; private set; }

    public TravelHistoryFileStatus Status { get; private set; }

    public DateTimeOffset? DateUploaded { get; private set; }

    public static TravelHistoryFile Create(
        long cardId,
        string fileName,
        string contentType,
        string blobContainer,
        string blobKey,
        string? displayName = null)
    {
        return new TravelHistoryFile
        {
            CardId = cardId,
            FileName = ValidFileName(fileName),
            DisplayName = ValidDisplayName(displayName),
            ContentType = ValidContentType(contentType),
            BlobContainer = ValidBlobContainer(blobContainer),
            BlobKey = ValidBlobKey(blobKey),
            Status = TravelHistoryFileStatus.Pending,
        };
    }

    public void Update(string fileName, string? displayName)
    {
        FileName = ValidFileName(fileName);
        DisplayName = ValidDisplayName(displayName);
    }

    public void MarkUploaded(long sizeInBytes)
    {
        Status = TravelHistoryFileStatus.Uploaded;
        SizeInBytes = Guard.Against.Negative(sizeInBytes, nameof(sizeInBytes));
        DateUploaded = DateTimeOffset.UtcNow;
    }

    private static string ValidFileName(string fileName)
    {
        return Guard.Against.ValidString(
            fileName,
            nameof(fileName),
            maxLength: 256,
            pattern: @"(?i)\.csv$",
            patternErrorMessage: "File name must be a .csv file.");
    }

    private static string? ValidDisplayName(string? displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            return null;
        }

        return Guard.Against.ValidString(displayName, nameof(displayName), maxLength: 256);
    }

    private static string ValidContentType(string contentType)
    {
        return Guard.Against.ValidString(contentType, nameof(contentType), maxLength: 128);
    }

    private static string ValidBlobContainer(string blobContainer)
    {
        return Guard.Against.ValidString(blobContainer, nameof(blobContainer), maxLength: 63);
    }

    private static string ValidBlobKey(string blobKey)
    {
        return Guard.Against.ValidString(blobKey, nameof(blobKey), maxLength: 512);
    }
}

namespace WebApi.Domain.Enums;

/// <summary>
/// Upload lifecycle of a <see cref="Entities.TravelHistoryFile"/>.
/// </summary>
public enum TravelHistoryFileStatus
{
    /// <summary>Metadata created; the blob has not been confirmed as uploaded yet.</summary>
    Pending = 0,

    /// <summary>The blob has been uploaded and confirmed.</summary>
    Uploaded = 1,
}

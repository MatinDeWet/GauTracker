namespace Pagination.Models.Requests;
/// <summary>
/// Represents a cursor-based pagination request.
/// </summary>
public class CursorPageableRequest : BasePaginationRequest
{
    /// <summary>
    /// Gets or sets the cursor. The cursor value from the previous page to continue pagination.
    /// For the first page, this should be null.
    /// </summary>
    public string? Cursor { get; set; }
}

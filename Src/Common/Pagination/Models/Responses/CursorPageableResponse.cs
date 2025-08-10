namespace Pagination.Models.Responses;
/// <summary>
/// Represents a cursor-based pagination response.
/// </summary>
/// <typeparam name="T">The type of the data elements.</typeparam>
public class CursorPageableResponse<T> : BasePaginationResponse<T>
{
    /// <summary>
    /// Gets or sets the next cursor. The cursor to use for the next page.
    /// If null, there are no more pages.
    /// </summary>
    public string? NextCursor { get; set; }

    /// <summary>
    /// Gets or sets the previous cursor. The cursor to use for the previous page.
    /// If null, this is the first page.
    /// </summary>
    public string? PreviousCursor { get; set; }

    /// <summary>
    /// Gets or sets whether there are more pages available.
    /// </summary>
    public bool HasNextPage { get; set; }

    /// <summary>
    /// Gets or sets whether there are previous pages available.
    /// </summary>
    public bool HasPreviousPage { get; set; }
}

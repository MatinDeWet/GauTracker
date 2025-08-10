namespace Pagination.Models.Responses;
/// <summary>
/// Represents an offset-based pagination response.
/// </summary>
/// <typeparam name="T">The type of the data elements.</typeparam>
public class PageableResponse<T> : BasePaginationResponse<T>
{
    /// <summary>
    /// Gets or sets the total records. The total number of records before pagination is applied.
    /// </summary>
    public int TotalRecords { get; set; }

    /// <summary>
    /// Gets or sets the page number. The Page Requested
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Gets or sets the page count. The total number of pages. Calculated from TotalRecords and PageSize.
    /// </summary>
    public int PageCount { get; set; }
}

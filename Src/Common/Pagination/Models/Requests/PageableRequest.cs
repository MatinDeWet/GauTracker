namespace Pagination.Models.Requests;
/// <summary>
/// Represents an offset-based pagination request.
/// </summary>
public abstract class PageableRequest : BasePaginationRequest
{
    /// <summary>
    /// Gets or sets the page number.
    /// </summary>
    public int PageNumber { get; set; } = 1;
}

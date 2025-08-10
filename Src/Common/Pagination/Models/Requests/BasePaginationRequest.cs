using Pagination.Enums;

namespace Pagination.Models.Requests;
/// <summary>
/// Base class for pagination requests with common ordering properties.
/// </summary>
public abstract class BasePaginationRequest
{
    /// <summary>
    /// Gets or sets the page size. Number of records to return.
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Gets or sets the order by. Referring to the property name of the entity.
    /// </summary>
    public string? OrderBy { get; set; }

    /// <summary>
    /// Gets or sets the order direction. Default is ascending.
    /// </summary>
    public OrderDirectionEnum OrderDirection { get; set; } = OrderDirectionEnum.Ascending;
}

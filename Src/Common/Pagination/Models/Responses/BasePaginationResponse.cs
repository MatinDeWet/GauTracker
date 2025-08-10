using Pagination.Enums;

namespace Pagination.Models.Responses;
/// <summary>
/// Base class for pagination responses with common properties.
/// </summary>
/// <typeparam name="T">The type of the data elements.</typeparam>
public abstract class BasePaginationResponse<T>
{
    /// <summary>
    /// Gets or sets the data. The collection of entities.
    /// </summary>
    public IEnumerable<T> Data { get; set; } = null!;

    /// <summary>
    /// Gets or sets the page size. The number of records requested.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Gets or sets the order by. The property name of the entity.
    /// </summary>
    public string OrderBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the order direction.
    /// </summary>
    public OrderDirectionEnum OrderDirection { get; set; }
}

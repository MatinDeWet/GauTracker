using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Pagination.Enums;
using Pagination.Models.Requests;
using Pagination.Models.Responses;

namespace Pagination;
/// <summary>
/// Provides extension methods for converting queryable sequences into pageable responses.
/// </summary>
public static class PageableExtensions
{
    /// <summary>
    /// Asynchronously converts an ordered query into a pageable response based on the provided paging request.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the query.</typeparam>
    /// <param name="query">The ordered queryable data source.</param>
    /// <param name="request">
    /// The paging request containing the page number, page size, order by field, and order direction.
    /// PageNumber and PageSize must be greater than 0.
    /// </param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation, with a result of a <see cref="PageableResponse{T}"/>
    /// containing the paged data and metadata.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the PageNumber or PageSize are less than or equal to 0.
    /// </exception>
    public static async Task<PageableResponse<T>> ToPageableListAsync<T>(this IOrderedQueryable<T> query, PageableRequest request, CancellationToken cancellationToken)
    {
        ValidatePageableRequest(request);

        int totalRecords = await query.CountAsync(cancellationToken)
            .ConfigureAwait(false);

        IQueryable<T> pageQuery = query.AsQueryable();

        int start = (request.PageNumber - 1) * request.PageSize;
        int pageCount = (int)Math.Ceiling(totalRecords / (double)request.PageSize);

        pageQuery = pageQuery.Skip(start);
        pageQuery = pageQuery.Take(request.PageSize);

        var result = new PageableResponse<T>
        {
            Data = await pageQuery.ToListAsync(cancellationToken).ConfigureAwait(false),
            PageSize = request.PageSize,
            PageNumber = request.PageNumber,
            PageCount = pageCount,
            TotalRecords = totalRecords,

            OrderDirection = request.OrderDirection,
            OrderBy = request.OrderBy ?? string.Empty,
        };

        return result;
    }

    /// <summary>
    /// Asynchronously converts a query into a pageable response by ordering the data using the specified OrderBy field.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the query.</typeparam>
    /// <param name="query">The queryable data source.</param>
    /// <param name="request">
    /// The paging request containing the page number, page size, order by field, and order direction.
    /// The OrderBy property must not be null, empty, or whitespace.
    /// </param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation, with a result of a <see cref="PageableResponse{T}"/>
    /// containing the paged data and metadata.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the OrderBy property of the request is null, empty, or whitespace.
    /// </exception>
    public static Task<PageableResponse<T>> ToPageableListAsync<T>(this IQueryable<T> query, PageableRequest request, CancellationToken cancellationToken)
    {
        ValidatePageableRequest(request);

        ArgumentException.ThrowIfNullOrWhiteSpace(request.OrderBy, nameof(request.OrderBy));

        if (request.OrderDirection == OrderDirectionEnum.Ascending)
        {
            return query.OrderBy(request.OrderBy).ToPageableListAsync(request, cancellationToken);
        }
        else
        {
            return query.OrderByDescending(request.OrderBy).ToPageableListAsync(request, cancellationToken);
        }
    }

    /// <summary>
    /// Asynchronously converts a query into a pageable response using a key selector for ordering when no explicit OrderBy is provided.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the query.</typeparam>
    /// <typeparam name="TKey">The type of the key used for ordering.</typeparam>
    /// <param name="query">The queryable data source.</param>
    /// <param name="orderKeySelector">
    /// An expression that selects the key used for ordering the data.
    /// This parameter is used only when the OrderBy property is not provided in the request.
    /// </param>
    /// <param name="request">
    /// The paging request containing the page number, page size, and order direction.
    /// </param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation, with a result of a <see cref="PageableResponse{T}"/>
    /// containing the paged data and metadata.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the orderKeySelector is null when no OrderBy value is provided.
    /// </exception>
    public static Task<PageableResponse<T>> ToPageableListAsync<T, TKey>(
        this IQueryable<T> query,
        Expression<Func<T, TKey>> orderKeySelector,
        OrderDirectionEnum orderDirection,
        PageableRequest request,
        CancellationToken cancellationToken)
    {
        ValidatePageableRequest(request);

        if (!string.IsNullOrWhiteSpace(request.OrderBy))
        {
            return query.ToPageableListAsync(request, cancellationToken);
        }

        ArgumentNullException.ThrowIfNull(orderKeySelector, nameof(orderKeySelector));

        if (orderDirection == OrderDirectionEnum.Ascending)
        {
            return query.OrderBy(orderKeySelector).ToPageableListAsync(request, cancellationToken);
        }
        else
        {
            return query.OrderByDescending(orderKeySelector).ToPageableListAsync(request, cancellationToken);
        }
    }

    /// <summary>
    /// Orders the elements of a sequence in ascending order according to the specified property name.
    /// </summary>
    /// <typeparam name="T">The type of the elements of the source sequence.</typeparam>
    /// <param name="source">The queryable data source.</param>
    /// <param name="propertyName">The name of the property to use for ordering.</param>
    /// <returns>
    /// An <see cref="IOrderedQueryable{T}"/> whose elements are sorted in ascending order by the specified property.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the specified property does not exist on type T.
    /// </exception>
    public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName)
    {
        return source.OrderBy(ToLambda<T>(propertyName));
    }

    /// <summary>
    /// Orders the elements of a sequence in descending order according to the specified property name.
    /// </summary>
    /// <typeparam name="T">The type of the elements of the source sequence.</typeparam>
    /// <param name="source">The queryable data source.</param>
    /// <param name="propertyName">The name of the property to use for ordering in descending order.</param>
    /// <returns>
    /// An <see cref="IOrderedQueryable{T}"/> whose elements are sorted in descending order by the specified property.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the specified property does not exist on type T.
    /// </exception>
    public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string propertyName)
    {
        return source.OrderByDescending(ToLambda<T>(propertyName));
    }

    /// <summary>
    /// Creates a lambda expression to access a specified property of an object.
    /// </summary>
    /// <typeparam name="T">The type of the object that contains the property.</typeparam>
    /// <param name="propertyName">The name of the property to access.</param>
    /// <returns>
    /// An expression representing a lambda that accesses the specified property, returning it as an object.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the specified property does not exist on type T.
    /// </exception>
    private static Expression<Func<T, object>> ToLambda<T>(string propertyName)
    {
        try
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T));
            MemberExpression property = Expression.Property(parameter, propertyName);

            // Handle value types properly by boxing them
            Expression propAsObject = property.Type.IsValueType
                ? Expression.Convert(property, typeof(object))
                : property;

            return Expression.Lambda<Func<T, object>>(propAsObject, parameter);
        }
        catch (ArgumentException ex)
        {
            throw new ArgumentException($"Property '{propertyName}' does not exist on type '{typeof(T).Name}'.", nameof(propertyName), ex);
        }
    }

    private static void ValidatePageableRequest(PageableRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(request.PageNumber, 0, nameof(request.PageNumber));
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(request.PageSize, 0, nameof(request.PageSize));
    }

    #region Cursor Pagination

    /// <summary>
    /// Asynchronously converts an ordered query into a cursor-based pageable response.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the query.</typeparam>
    /// <param name="query">The ordered queryable data source.</param>
    /// <param name="request">The cursor-based paging request.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation, with a result of a <see cref="CursorPageableResponse{T}"/>
    /// containing the paged data and cursor metadata.
    /// </returns>
    public static async Task<CursorPageableResponse<T>> ToCursorPageableListAsync<T>(
        this IOrderedQueryable<T> query,
        CursorPageableRequest request,
        CancellationToken cancellationToken)
    {
        ValidateCursorPageableRequest(request);

        IQueryable<T> pageQuery = query.AsQueryable();

        // Apply cursor filtering if provided
        if (!string.IsNullOrWhiteSpace(request.Cursor))
        {
            object cursorValue = DecodeCursor(request.Cursor);
            pageQuery = ApplyCursorFilter(pageQuery, request.OrderBy ?? string.Empty, cursorValue, request.OrderDirection);
        }

        // Take one extra record to determine if there's a next page
        List<T> results = await pageQuery.Take(request.PageSize + 1)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        bool hasNextPage = results.Count > request.PageSize;
        IEnumerable<T> data = hasNextPage ? results.Take(request.PageSize) : results;

        CursorPageableResponse<T> response = new()
        {
            Data = data,
            PageSize = request.PageSize,
            HasNextPage = hasNextPage,
            HasPreviousPage = !string.IsNullOrWhiteSpace(request.Cursor),
            OrderBy = request.OrderBy ?? string.Empty,
            OrderDirection = request.OrderDirection
        };

        // Set cursors
        var dataList = data.ToList();
        if (dataList.Count > 0)
        {
            if (hasNextPage)
            {
                T lastItem = dataList.Last();
                response.NextCursor = EncodeCursor(GetPropertyValue(lastItem, request.OrderBy ?? string.Empty));
            }

            if (!string.IsNullOrWhiteSpace(request.Cursor))
            {
                // For previous cursor, we would need to implement reverse pagination
                // This is a simplified version - in a real implementation, you might want to store the previous cursor
                response.PreviousCursor = null;
            }
        }

        return response;
    }

    /// <summary>
    /// Asynchronously converts a query into a cursor-based pageable response by ordering the data using the specified OrderBy field.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the query.</typeparam>
    /// <param name="query">The queryable data source.</param>
    /// <param name="request">The cursor-based paging request. The OrderBy property must not be null, empty, or whitespace.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation, with a result of a <see cref="CursorPageableResponse{T}"/>
    /// containing the paged data and cursor metadata.
    /// </returns>
    public static Task<CursorPageableResponse<T>> ToCursorPageableListAsync<T>(
        this IQueryable<T> query,
        CursorPageableRequest request,
        CancellationToken cancellationToken)
    {
        ValidateCursorPageableRequest(request);

        ArgumentException.ThrowIfNullOrWhiteSpace(request.OrderBy, nameof(request.OrderBy));

        if (request.OrderDirection == OrderDirectionEnum.Ascending)
        {
            return query.OrderBy(request.OrderBy).ToCursorPageableListAsync(request, cancellationToken);
        }
        else
        {
            return query.OrderByDescending(request.OrderBy).ToCursorPageableListAsync(request, cancellationToken);
        }
    }

    /// <summary>
    /// Asynchronously converts a query into a cursor-based pageable response using a key selector for ordering.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the query.</typeparam>
    /// <typeparam name="TKey">The type of the key used for ordering.</typeparam>
    /// <param name="query">The queryable data source.</param>
    /// <param name="orderKeySelector">An expression that selects the key used for ordering the data.</param>
    /// <param name="request">The cursor-based paging request.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation, with a result of a <see cref="CursorPageableResponse{T}"/>
    /// containing the paged data and cursor metadata.
    /// </returns>
    public static Task<CursorPageableResponse<T>> ToCursorPageableListAsync<T, TKey>(
        this IQueryable<T> query,
        Expression<Func<T, TKey>> orderKeySelector,
        CursorPageableRequest request,
        CancellationToken cancellationToken)
    {
        ValidateCursorPageableRequest(request);

        ArgumentNullException.ThrowIfNull(orderKeySelector, nameof(orderKeySelector));

        if (request.OrderDirection == OrderDirectionEnum.Ascending)
        {
            return query.OrderBy(orderKeySelector).ToCursorPageableListAsync(request, cancellationToken);
        }
        else
        {
            return query.OrderByDescending(orderKeySelector).ToCursorPageableListAsync(request, cancellationToken);
        }
    }

    #endregion

    #region Private Cursor Helpers

    /// <summary>
    /// Applies cursor filtering to the query based on the cursor value and ordering.
    /// </summary>
    private static IQueryable<T> ApplyCursorFilter<T>(IQueryable<T> query, string orderBy, object cursorValue, OrderDirectionEnum orderDirection)
    {
        ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
        MemberExpression property = Expression.Property(parameter, orderBy);
        ConstantExpression constant = Expression.Constant(cursorValue);

        // Convert constant to property type if needed
        Expression convertedConstant = property.Type != cursorValue.GetType()
            ? Expression.Convert(constant, property.Type)
            : constant;

        BinaryExpression comparison = orderDirection == OrderDirectionEnum.Ascending
            ? Expression.GreaterThan(property, convertedConstant)
            : Expression.LessThan(property, convertedConstant);

        var lambda = Expression.Lambda<Func<T, bool>>(comparison, parameter);
        return query.Where(lambda);
    }

    /// <summary>
    /// Gets the value of a property from an object using reflection.
    /// </summary>
    private static object GetPropertyValue<T>(T obj, string propertyName)
    {
        PropertyInfo? propertyInfo = typeof(T).GetProperty(propertyName);
        ArgumentNullException.ThrowIfNull(propertyInfo, $"Property '{propertyName}' does not exist on type '{typeof(T).Name}'.");
        return propertyInfo.GetValue(obj) ?? throw new InvalidOperationException($"Property '{propertyName}' value is null.");
    }

    /// <summary>
    /// Encodes a cursor value to a Base64 string.
    /// </summary>
    private static string EncodeCursor(object value)
    {
        string json = JsonSerializer.Serialize(value);
        byte[] bytes = Encoding.UTF8.GetBytes(json);
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Decodes a Base64 cursor string back to its original value.
    /// </summary>
    private static object DecodeCursor(string cursor)
    {
        try
        {
            byte[] bytes = Convert.FromBase64String(cursor);
            string json = Encoding.UTF8.GetString(bytes);

            // Use JsonDocument to parse and get the actual value type
            using var document = JsonDocument.Parse(json);
            JsonElement element = document.RootElement;

            return element.ValueKind switch
            {
                JsonValueKind.Number when element.TryGetInt32(out int intValue) => intValue,
                JsonValueKind.Number when element.TryGetInt64(out long longValue) => longValue,
                JsonValueKind.Number when element.TryGetDecimal(out decimal decimalValue) => decimalValue,
                JsonValueKind.Number when element.TryGetDouble(out double doubleValue) => doubleValue,
                JsonValueKind.String => element.GetString() ?? string.Empty,
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                _ => element.GetRawText()
            };
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Invalid cursor format.", nameof(cursor), ex);
        }
    }

    /// <summary>
    /// Validates the cursor pageable request parameters.
    /// </summary>
    private static void ValidateCursorPageableRequest(CursorPageableRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(request.PageSize, 0, nameof(request.PageSize));
    }

    #endregion
}

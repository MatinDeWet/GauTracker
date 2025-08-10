using System.Linq.Expressions;
using MockQueryable;
using Pagination.Enums;
using Pagination.Models.Responses;
using Pagination.UnitTests.Models;
using Shouldly;

namespace Pagination.UnitTests.Tests;
public class CursorPageableExtensionsTests
{
    private readonly IQueryable<TestEntity> _entities;

    public CursorPageableExtensionsTests()
    {
        // Create test data
        var testData = new List<TestEntity>
        {
            new() { Id = 1, Name = "Entity A", CreatedDate = new DateTime(2023, 1, 15), Value = 10.5m },
            new() { Id = 2, Name = "Entity C", CreatedDate = new DateTime(2023, 3, 20), Value = 20.1m },
            new() { Id = 3, Name = "Entity B", CreatedDate = new DateTime(2023, 2, 10), Value = 15.3m },
            new() { Id = 4, Name = "Entity E", CreatedDate = new DateTime(2023, 5, 5), Value = 30.7m },
            new() { Id = 5, Name = "Entity D", CreatedDate = new DateTime(2023, 4, 1), Value = 25.9m },
            new() { Id = 6, Name = "Entity F", CreatedDate = new DateTime(2023, 6, 12), Value = 35.2m },
            new() { Id = 7, Name = "Entity H", CreatedDate = new DateTime(2023, 8, 8), Value = 45.6m },
            new() { Id = 8, Name = "Entity G", CreatedDate = new DateTime(2023, 7, 22), Value = 40.3m },
            new() { Id = 9, Name = "Entity J", CreatedDate = new DateTime(2023, 10, 30), Value = 55.8m },
            new() { Id = 10, Name = "Entity I", CreatedDate = new DateTime(2023, 9, 18), Value = 50.4m }
        };

        // Set up a mock queryable source
        _entities = testData.BuildMock();
    }

    [Fact]
    public async Task ToCursorPageableListAsync_WhenPageSizeIsLessThanOne_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var request = new TestCursorPageableRequest
        {
            PageSize = 0,
            OrderBy = "Id",
            OrderDirection = OrderDirectionEnum.Ascending
        };
        IOrderedQueryable<TestEntity> orderedQuery = _entities.OrderBy(e => e.Id);

        // Act & Assert
        ArgumentOutOfRangeException exception = await Should.ThrowAsync<ArgumentOutOfRangeException>(
            orderedQuery.ToCursorPageableListAsync(request, CancellationToken.None));
        exception.ParamName.ShouldBe("PageSize");
    }

    [Fact]
    public async Task ToCursorPageableListAsync_WhenRequestIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        IOrderedQueryable<TestEntity> orderedQuery = _entities.OrderBy(e => e.Id);

        // Act & Assert
        ArgumentNullException exception = await Should.ThrowAsync<ArgumentNullException>(
            orderedQuery.ToCursorPageableListAsync(null!, CancellationToken.None));
        exception.ParamName.ShouldBe("request");
    }

    [Fact]
    public async Task ToCursorPageableListAsync_FirstPage_ReturnsCorrectData()
    {
        // Arrange
        var request = new TestCursorPageableRequest
        {
            PageSize = 3,
            OrderBy = "Id",
            OrderDirection = OrderDirectionEnum.Ascending
        };
        IOrderedQueryable<TestEntity> orderedQuery = _entities.OrderBy(e => e.Id);

        // Act
        CursorPageableResponse<TestEntity> result = await orderedQuery.ToCursorPageableListAsync(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Data.Count().ShouldBe(3);
        result.PageSize.ShouldBe(3);
        result.HasNextPage.ShouldBeTrue();
        result.HasPreviousPage.ShouldBeFalse();
        result.NextCursor.ShouldNotBeNull();
        result.PreviousCursor.ShouldBeNull();
        result.OrderBy.ShouldBe("Id");
        result.OrderDirection.ShouldBe(OrderDirectionEnum.Ascending);

        var dataList = result.Data.ToList();
        dataList[0].Id.ShouldBe(1);
        dataList[1].Id.ShouldBe(2);
        dataList[2].Id.ShouldBe(3);
    }

    [Fact]
    public async Task ToCursorPageableListAsync_WithCursor_ReturnsCorrectData()
    {
        // Arrange
        var firstPageRequest = new TestCursorPageableRequest
        {
            PageSize = 3,
            OrderBy = "Id",
            OrderDirection = OrderDirectionEnum.Ascending
        };
        IOrderedQueryable<TestEntity> orderedQuery = _entities.OrderBy(e => e.Id);

        // Get first page to get the cursor
        CursorPageableResponse<TestEntity> firstPage = await orderedQuery.ToCursorPageableListAsync(firstPageRequest, CancellationToken.None);

        var secondPageRequest = new TestCursorPageableRequest
        {
            PageSize = 3,
            OrderBy = "Id",
            OrderDirection = OrderDirectionEnum.Ascending,
            Cursor = firstPage.NextCursor
        };

        // Act
        CursorPageableResponse<TestEntity> result = await orderedQuery.ToCursorPageableListAsync(secondPageRequest, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Data.Count().ShouldBe(3);
        result.PageSize.ShouldBe(3);
        result.HasNextPage.ShouldBeTrue();
        result.HasPreviousPage.ShouldBeTrue();
        result.NextCursor.ShouldNotBeNull();

        var dataList = result.Data.ToList();
        dataList[0].Id.ShouldBe(4);
        dataList[1].Id.ShouldBe(5);
        dataList[2].Id.ShouldBe(6);
    }

    [Fact]
    public async Task ToCursorPageableListAsync_LastPage_HasNoNextPage()
    {
        // Arrange
        var request = new TestCursorPageableRequest
        {
            PageSize = 15, // Larger than total records
            OrderBy = "Id",
            OrderDirection = OrderDirectionEnum.Ascending
        };
        IOrderedQueryable<TestEntity> orderedQuery = _entities.OrderBy(e => e.Id);

        // Act
        CursorPageableResponse<TestEntity> result = await orderedQuery.ToCursorPageableListAsync(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Data.Count().ShouldBe(10); // All records
        result.HasNextPage.ShouldBeFalse();
        result.NextCursor.ShouldBeNull();
    }

    [Fact]
    public async Task ToCursorPageableListAsync_WithIQueryable_OrdersByProperty()
    {
        // Arrange
        var request = new TestCursorPageableRequest
        {
            PageSize = 3,
            OrderBy = "Name",
            OrderDirection = OrderDirectionEnum.Ascending
        };

        // Act
        CursorPageableResponse<TestEntity> result = await _entities.ToCursorPageableListAsync(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Data.Count().ShouldBe(3);

        var dataList = result.Data.ToList();
        dataList[0].Name.ShouldBe("Entity A");
        dataList[1].Name.ShouldBe("Entity B");
        dataList[2].Name.ShouldBe("Entity C");
    }

    [Fact]
    public async Task ToCursorPageableListAsync_WithKeySelector_OrdersByExpression()
    {
        // Arrange
        var request = new TestCursorPageableRequest
        {
            PageSize = 3,
            OrderBy = "Value",
            OrderDirection = OrderDirectionEnum.Descending
        };

        // Act
        CursorPageableResponse<TestEntity> result = await _entities.ToCursorPageableListAsync(e => e.Value, request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Data.Count().ShouldBe(3);

        var dataList = result.Data.ToList();
        dataList[0].Value.ShouldBe(55.8m); // Entity J
        dataList[1].Value.ShouldBe(50.4m); // Entity I
        dataList[2].Value.ShouldBe(45.6m); // Entity H
    }

    [Fact]
    public async Task ToCursorPageableListAsync_WithNullOrderBy_ThrowsArgumentException()
    {
        // Arrange
        var request = new TestCursorPageableRequest
        {
            PageSize = 3,
            OrderBy = null!,
            OrderDirection = OrderDirectionEnum.Ascending
        };

        // Act & Assert
        ArgumentNullException exception = await Should.ThrowAsync<ArgumentNullException>(() =>
            _entities.ToCursorPageableListAsync(request, CancellationToken.None));
        exception.ParamName.ShouldBe("OrderBy");
    }

    [Fact]
    public async Task ToCursorPageableListAsync_WithEmptyOrderBy_ThrowsArgumentException()
    {
        // Arrange
        var request = new TestCursorPageableRequest
        {
            PageSize = 3,
            OrderBy = "",
            OrderDirection = OrderDirectionEnum.Ascending
        };

        // Act & Assert
        ArgumentException exception = await Should.ThrowAsync<ArgumentException>(() => _entities.ToCursorPageableListAsync(request, CancellationToken.None));
        exception.ParamName.ShouldBe("OrderBy");
    }

    [Fact]
    public async Task ToCursorPageableListAsync_WithWhitespaceOrderBy_ThrowsArgumentException()
    {
        // Arrange
        var request = new TestCursorPageableRequest
        {
            PageSize = 3,
            OrderBy = "   ",
            OrderDirection = OrderDirectionEnum.Ascending
        };

        // Act & Assert
        ArgumentException exception = await Should.ThrowAsync<ArgumentException>(() => _entities.ToCursorPageableListAsync(request, CancellationToken.None));
        exception.ParamName.ShouldBe("OrderBy");
    }

    [Fact]
    public async Task ToCursorPageableListAsync_WithDescendingOrder_ReturnsCorrectData()
    {
        // Arrange
        var request = new TestCursorPageableRequest
        {
            PageSize = 3,
            OrderBy = "Id",
            OrderDirection = OrderDirectionEnum.Descending
        };

        // Act
        CursorPageableResponse<TestEntity> result = await _entities.ToCursorPageableListAsync(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Data.Count().ShouldBe(3);
        result.OrderDirection.ShouldBe(OrderDirectionEnum.Descending);

        var dataList = result.Data.ToList();
        dataList[0].Id.ShouldBe(10);
        dataList[1].Id.ShouldBe(9);
        dataList[2].Id.ShouldBe(8);
    }

    [Fact]
    public async Task ToCursorPageableListAsync_WithNullKeySelector_ThrowsArgumentNullException()
    {
        // Arrange
        var request = new TestCursorPageableRequest
        {
            PageSize = 3,
            OrderBy = "Id",
            OrderDirection = OrderDirectionEnum.Ascending
        };

        // Act & Assert
        ArgumentNullException exception = await Should.ThrowAsync<ArgumentNullException>(() => _entities.ToCursorPageableListAsync((Expression<Func<TestEntity, object>>)null!, request, CancellationToken.None));
        exception.ParamName.ShouldBe("orderKeySelector");
    }
}

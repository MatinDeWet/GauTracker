using MockQueryable;
using Pagination.Enums;
using Pagination.Models.Responses;
using Pagination.UnitTests.Models;
using Shouldly;

namespace Pagination.UnitTests.Tests;
public class PageableExtensionsTests
{
    private readonly IQueryable<TestEntity> _entities;

    public PageableExtensionsTests()
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
    public async Task ToPageableListAsync_WhenPageNumberIsLessThanOne_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var request = new TestPageableRequest
        {
            PageNumber = 0,
            PageSize = 5,
            OrderBy = "Id",
            OrderDirection = OrderDirectionEnum.Ascending
        };
        IOrderedQueryable<TestEntity> orderedQuery = _entities.OrderBy(e => e.Id);

        // Act & Assert
        await Should.ThrowAsync<ArgumentOutOfRangeException>(() =>
            orderedQuery.ToPageableListAsync(request, CancellationToken.None));
    }

    [Fact]
    public async Task ToPageableListAsync_WhenPageSizeIsLessThanOne_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var request = new TestPageableRequest
        {
            PageNumber = 1,
            PageSize = 0,
            OrderBy = "Id",
            OrderDirection = OrderDirectionEnum.Ascending
        };
        IOrderedQueryable<TestEntity> orderedQuery = _entities.OrderBy(e => e.Id);

        // Act & Assert
        await Should.ThrowAsync<ArgumentOutOfRangeException>(() =>
            orderedQuery.ToPageableListAsync(request, CancellationToken.None));
    }

    [Fact]
    public async Task ToPageableListAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        IOrderedQueryable<TestEntity> orderedQuery = _entities.OrderBy(e => e.Id);

        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(() =>
            orderedQuery.ToPageableListAsync(null!, CancellationToken.None));
    }

    [Fact]
    public async Task ToPageableListAsync_WhenPageNumberIsOne_ReturnsFirstPage()
    {
        // Arrange
        var request = new TestPageableRequest
        {
            PageNumber = 1,
            PageSize = 3,
            OrderBy = "Id",
            OrderDirection = OrderDirectionEnum.Ascending
        };
        IOrderedQueryable<TestEntity> orderedQuery = _entities.OrderBy(e => e.Id);

        // Act
        PageableResponse<TestEntity> result = await orderedQuery.ToPageableListAsync(request, CancellationToken.None);

        // Assert
        result.TotalRecords.ShouldBe(10);
        result.PageSize.ShouldBe(3);
        result.PageNumber.ShouldBe(1);
        result.PageCount.ShouldBe(4);
        result.Data.Count().ShouldBe(3);
        result.OrderBy.ShouldBe("Id");
        result.OrderDirection.ShouldBe(OrderDirectionEnum.Ascending);

        // Verify first page content
        var data = result.Data.ToList();
        data[0].Id.ShouldBe(1);
        data[1].Id.ShouldBe(2);
        data[2].Id.ShouldBe(3);
    }

    [Fact]
    public async Task ToPageableListAsync_WhenPageNumberIsTwo_ReturnsSecondPage()
    {
        // Arrange
        var request = new TestPageableRequest
        {
            PageNumber = 2,
            PageSize = 3,
            OrderBy = "Id",
            OrderDirection = OrderDirectionEnum.Ascending
        };
        IOrderedQueryable<TestEntity> orderedQuery = _entities.OrderBy(e => e.Id);

        // Act
        PageableResponse<TestEntity> result = await orderedQuery.ToPageableListAsync(request, CancellationToken.None);

        // Assert
        result.Data.Count().ShouldBe(3);

        // Verify second page content
        var data = result.Data.ToList();
        data[0].Id.ShouldBe(4);
        data[1].Id.ShouldBe(5);
        data[2].Id.ShouldBe(6);
    }

    [Fact]
    public async Task ToPageableListAsync_WithLastPage_ReturnsRemainingItems()
    {
        // Arrange
        var request = new TestPageableRequest
        {
            PageNumber = 4,
            PageSize = 3,
            OrderBy = "Id",
            OrderDirection = OrderDirectionEnum.Ascending
        };
        IOrderedQueryable<TestEntity> orderedQuery = _entities.OrderBy(e => e.Id);

        // Act
        PageableResponse<TestEntity> result = await orderedQuery.ToPageableListAsync(request, CancellationToken.None);

        // Assert
        result.Data.ShouldHaveSingleItem();
        result.Data.First().Id.ShouldBe(10);
    }

    [Fact]
    public async Task ToPageableListAsync_WithOrderByNullOrEmpty_ThrowsArgumentException()
    {
        // Arrange
        var request = new TestPageableRequest
        {
            PageNumber = 1,
            PageSize = 5,
            OrderBy = null!,
            OrderDirection = OrderDirectionEnum.Ascending
        };

        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(() =>
            _entities.ToPageableListAsync(request, CancellationToken.None));

        request.OrderBy = "";
        await Should.ThrowAsync<ArgumentException>(() =>
            _entities.ToPageableListAsync(request, CancellationToken.None));

        request.OrderBy = "   ";
        await Should.ThrowAsync<ArgumentException>(() =>
            _entities.ToPageableListAsync(request, CancellationToken.None));
    }

    [Fact]
    public async Task ToPageableListAsync_WithAscendingOrder_OrdersDataAscending()
    {
        // Arrange
        var request = new TestPageableRequest
        {
            PageNumber = 1,
            PageSize = 10,
            OrderBy = "Name",
            OrderDirection = OrderDirectionEnum.Ascending
        };

        // Act
        PageableResponse<TestEntity> result = await _entities.ToPageableListAsync(request, CancellationToken.None);

        // Assert
        var data = result.Data.ToList();
        for (int i = 1; i < data.Count; i++)
        {
            (string.Compare(data[i - 1].Name, data[i].Name, StringComparison.Ordinal) <= 0).ShouldBeTrue();
        }
    }

    [Fact]
    public async Task ToPageableListAsync_WithDescendingOrder_OrdersDataDescending()
    {
        // Arrange
        var request = new TestPageableRequest
        {
            PageNumber = 1,
            PageSize = 10,
            OrderBy = "Value",
            OrderDirection = OrderDirectionEnum.Descending
        };

        // Act
        PageableResponse<TestEntity> result = await _entities.ToPageableListAsync(request, CancellationToken.None);

        // Assert
        var data = result.Data.ToList();
        for (int i = 1; i < data.Count; i++)
        {
            (data[i - 1].Value >= data[i].Value).ShouldBeTrue();
        }
    }

    [Fact]
    public async Task ToPageableListAsync_WithEmptyResults_ReturnsEmptyPage()
    {
        // Arrange
        var request = new TestPageableRequest
        {
            PageNumber = 1,
            PageSize = 10,
            OrderBy = "Id",
            OrderDirection = OrderDirectionEnum.Ascending
        };
        IQueryable<TestEntity> emptyQuery = _entities.Where(e => e.Id > 100);

        // Act
        PageableResponse<TestEntity> result = await emptyQuery.OrderBy(e => e.Id).ToPageableListAsync(request, CancellationToken.None);

        // Assert
        result.Data.ShouldBeEmpty();
        result.TotalRecords.ShouldBe(0);
        result.PageCount.ShouldBe(0);
        result.PageNumber.ShouldBe(1);
        result.PageSize.ShouldBe(10);
    }

    [Fact]
    public async Task ToPageableListAsync_WithKeySelectorAndNoOrderBy_UsesKeySelector()
    {
        // Arrange
        var request = new TestPageableRequest
        {
            PageNumber = 1,
            PageSize = 10,
            OrderBy = "",
            OrderDirection = OrderDirectionEnum.Ascending
        };

        // Act & Assert - first verify it throws when no OrderBy and no KeySelector
        await Should.ThrowAsync<ArgumentException>(() =>
            _entities.ToPageableListAsync(request, CancellationToken.None));

        // Then test with KeySelector
        PageableResponse<TestEntity> result = await _entities.ToPageableListAsync(e => e.CreatedDate, OrderDirectionEnum.Ascending, request, CancellationToken.None);

        // Assert data is ordered by CreatedDate
        var data = result.Data.ToList();
        for (int i = 1; i < data.Count; i++)
        {
            (data[i - 1].CreatedDate <= data[i].CreatedDate).ShouldBeTrue();
        }
    }

    [Fact]
    public async Task ToPageableListAsync_WithKeySelectorAndOrderBy_UsesOrderBy()
    {
        // Arrange
        var request = new TestPageableRequest
        {
            PageNumber = 1,
            PageSize = 10,
            OrderBy = "Name",
            OrderDirection = OrderDirectionEnum.Ascending
        };

        // Act - provide a different KeySelector than OrderBy
        PageableResponse<TestEntity> result = await _entities.ToPageableListAsync(e => e.Id, OrderDirectionEnum.Ascending, request, CancellationToken.None);

        // Assert data is ordered by Name (from OrderBy), not Id (from KeySelector)
        var data = result.Data.ToList();
        for (int i = 1; i < data.Count; i++)
        {
            (string.Compare(data[i - 1].Name, data[i].Name, StringComparison.Ordinal) <= 0).ShouldBeTrue();
        }
    }

    [Fact]
    public async Task ToPageableListAsync_WithNullKeySelector_ThrowsArgumentNullException()
    {
        // Arrange
        var request = new TestPageableRequest
        {
            PageNumber = 1,
            PageSize = 10,
            OrderBy = "",
            OrderDirection = OrderDirectionEnum.Ascending
        };

        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(() =>
            _entities.ToPageableListAsync<TestEntity, int>(null!, OrderDirectionEnum.Ascending, request, CancellationToken.None));
    }

    [Fact]
    public async Task ToPageableListAsync_WhenPageNumberExceedsAvailablePages_ReturnsEmptyData()
    {
        // Arrange
        var request = new TestPageableRequest
        {
            PageNumber = 100, // Way beyond available pages
            PageSize = 5,
            OrderBy = "Id",
            OrderDirection = OrderDirectionEnum.Ascending
        };

        // Act
        PageableResponse<TestEntity> result = await _entities.ToPageableListAsync(request, CancellationToken.None);

        // Assert
        result.Data.ShouldBeEmpty();
        result.TotalRecords.ShouldBe(10); // Total records should still be correct
        result.PageCount.ShouldBe(2); // Correct page count
        result.PageNumber.ShouldBe(100); // Requested page number preserved
        result.PageSize.ShouldBe(5);
    }

    [Fact]
    public async Task ToPageableListAsync_WithDateTimeOrdering_OrdersCorrectly()
    {
        // Arrange
        var request = new TestPageableRequest
        {
            PageNumber = 1,
            PageSize = 10,
            OrderBy = "CreatedDate",
            OrderDirection = OrderDirectionEnum.Ascending
        };

        // Act
        PageableResponse<TestEntity> result = await _entities.ToPageableListAsync(request, CancellationToken.None);

        // Assert
        var data = result.Data.ToList();
        for (int i = 1; i < data.Count; i++)
        {
            (data[i - 1].CreatedDate <= data[i].CreatedDate).ShouldBeTrue();
        }
    }

    [Fact]
    public async Task ToPageableListAsync_WithDecimalOrdering_OrdersCorrectly()
    {
        // Arrange
        var request = new TestPageableRequest
        {
            PageNumber = 1,
            PageSize = 10,
            OrderBy = "Value",
            OrderDirection = OrderDirectionEnum.Ascending
        };

        // Act
        PageableResponse<TestEntity> result = await _entities.ToPageableListAsync(request, CancellationToken.None);

        // Assert
        var data = result.Data.ToList();
        for (int i = 1; i < data.Count; i++)
        {
            (data[i - 1].Value <= data[i].Value).ShouldBeTrue();
        }
    }

    [Fact]
    public async Task ToPageableListAsync_WithInvalidPropertyName_ThrowsArgumentException()
    {
        // Arrange
        var request = new TestPageableRequest
        {
            PageNumber = 1,
            PageSize = 5,
            OrderBy = "NonExistentProperty",
            OrderDirection = OrderDirectionEnum.Ascending
        };

        // Act & Assert
        ArgumentException exception = await Should.ThrowAsync<ArgumentException>(() =>
            _entities.ToPageableListAsync(request, CancellationToken.None));

        exception.Message.ShouldContain("NonExistentProperty");
        exception.Message.ShouldContain("TestEntity");
    }

    [Fact]
    public void OrderBy_ShouldOrderBySpecifiedProperty()
    {
        // Act
        var result = _entities.OrderBy("Name").ToList();

        // Assert
        var expected = result.OrderBy(e => e.Name).ToList();
        result.ShouldBe(expected);
    }

    [Fact]
    public void OrderByDescending_ShouldOrderBySpecifiedPropertyDescending()
    {
        // Act
        var result = _entities.OrderByDescending("Value").ToList();

        // Assert
        var expected = result.OrderByDescending(e => e.Value).ToList();
        result.ShouldBe(expected);
    }

    [Fact]
    public void OrderBy_WhenPropertyDoesNotExist_ThrowsArgumentException()
    {
        // Act & Assert
        ArgumentException exception = Should.Throw<ArgumentException>(() =>
            _entities.OrderBy("NonExistentProperty").ToList());

        exception.Message.ShouldContain("NonExistentProperty");
        exception.Message.ShouldContain("TestEntity");
    }

    [Fact]
    public void OrderByDescending_WhenPropertyDoesNotExist_ThrowsArgumentException()
    {
        // Act & Assert
        ArgumentException exception = Should.Throw<ArgumentException>(() =>
            _entities.OrderByDescending("NonExistentProperty").ToList());

        exception.Message.ShouldContain("NonExistentProperty");
        exception.Message.ShouldContain("TestEntity");
    }

    [Fact]
    public async Task ToPageableListAsync_WithKeySelectorDescending_OrdersDataDescending()
    {
        // Arrange
        var request = new TestPageableRequest
        {
            PageNumber = 1,
            PageSize = 10,
            OrderBy = "", // Empty OrderBy to use keySelector
            OrderDirection = OrderDirectionEnum.Descending
        };

        // Act
        PageableResponse<TestEntity> result = await _entities.ToPageableListAsync(e => e.Value, OrderDirectionEnum.Descending, request, CancellationToken.None);

        // Assert data is ordered by Value descending
        var data = result.Data.ToList();
        for (int i = 1; i < data.Count; i++)
        {
            (data[i - 1].Value >= data[i].Value).ShouldBeTrue();
        }
    }

    [Fact]
    public async Task ToPageableListAsync_PageCountCalculation_IsCorrect()
    {
        // Arrange - Test various page size scenarios
        var testScenarios = new[]
        {
            new { PageSize = 3, ExpectedPageCount = 4 }, // 10 items / 3 per page = 3.33 → 4 pages
            new { PageSize = 5, ExpectedPageCount = 2 }, // 10 items / 5 per page = 2 pages
            new { PageSize = 10, ExpectedPageCount = 1 }, // 10 items / 10 per page = 1 page
            new { PageSize = 15, ExpectedPageCount = 1 } // 10 items / 15 per page = 0.67 → 1 page
        };

        foreach (var scenario in testScenarios)
        {
            var request = new TestPageableRequest
            {
                PageNumber = 1,
                PageSize = scenario.PageSize,
                OrderBy = "Id",
                OrderDirection = OrderDirectionEnum.Ascending
            };

            // Act
            PageableResponse<TestEntity> result = await _entities.ToPageableListAsync(request, CancellationToken.None);

            // Assert
            result.PageCount.ShouldBe(scenario.ExpectedPageCount);
        }
    }
}

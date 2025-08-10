using Microsoft.EntityFrameworkCore;
using Moq;
using Repository.Core.Implementation;
using Repository.UnitTests.Models;
using Shouldly;

namespace Repository.UnitTests.Tests;

public class BasicQueriesTests
{
    private readonly Mock<DbContext> _mockContext;
    private readonly QueryRepo<DbContext> _basicQueries;
    private readonly Mock<DbSet<TestEntity>> _mockDbSet;
    private readonly List<TestEntity> _testEntities;

    public BasicQueriesTests()
    {
        // Create test entities for the mock DbSet
        _testEntities =
        [
            new TestEntity { Id = 1, Name = "Entity 1", IsActive = true, CreatedDate = DateTime.UtcNow.AddDays(-2) },
            new TestEntity { Id = 2, Name = "Entity 2", IsActive = true, CreatedDate = DateTime.UtcNow.AddDays(-1) },
            new TestEntity { Id = 3, Name = "Entity 3", IsActive = false, CreatedDate = DateTime.UtcNow }
        ];

        // Set up queryable mock DbSet
        _mockDbSet = CreateMockDbSet(_testEntities);

        // Mock the DbContext
        _mockContext = new Mock<DbContext>();
        _mockContext.Setup(c => c.Set<TestEntity>())
            .Returns(_mockDbSet.Object);

        // Create QueryRepo instance to test
        _basicQueries = new QueryRepo<DbContext>(_mockContext.Object);
    }

    #region GetQueryable Method Tests

    [Fact]
    public void GetQueryable_ReturnsDbSetQueryable()
    {
        // Act
        IQueryable<TestEntity> result = _basicQueries.GetQueryable<TestEntity>();

        // Assert
        result.ShouldBeSameAs(_mockDbSet.Object);

        // Verify the context's Set<T>() method was called
        _mockContext.Verify(c => c.Set<TestEntity>(), Times.Once);
    }

    [Fact]
    public void GetQueryable_WithDifferentEntityType_ReturnsCorrectDbSet()
    {
        // Arrange
        var complexEntities = new List<ComplexTestEntity>
        {
            new() { Id = 1, Name = "Complex 1", Category = "A", IsActive = true },
            new() { Id = 2, Name = "Complex 2", Category = "B", IsActive = false }
        };

        Mock<DbSet<ComplexTestEntity>> complexMockDbSet = CreateMockDbSet(complexEntities);
        _mockContext.Setup(c => c.Set<ComplexTestEntity>())
            .Returns(complexMockDbSet.Object);

        // Act
        IQueryable<ComplexTestEntity> result = _basicQueries.GetQueryable<ComplexTestEntity>();

        // Assert
        result.ShouldBeSameAs(complexMockDbSet.Object);
        _mockContext.Verify(c => c.Set<ComplexTestEntity>(), Times.Once);
    }

    [Fact]
    public void GetQueryable_CalledMultipleTimes_EachCallInvokesContextSet()
    {
        // Act
        IQueryable<TestEntity> result1 = _basicQueries.GetQueryable<TestEntity>();
        IQueryable<TestEntity> result2 = _basicQueries.GetQueryable<TestEntity>();

        // Assert
        result1.ShouldBeSameAs(_mockDbSet.Object);
        result2.ShouldBeSameAs(_mockDbSet.Object);
        _mockContext.Verify(c => c.Set<TestEntity>(), Times.Exactly(2));
    }

    [Fact]
    public void GetQueryable_WithMultipleEntityTypes_CallsCorrectSetMethods()
    {
        // Arrange
        var otherEntities = new List<OtherTestEntity>
        {
            new() { Id = 1, Description = "Other 1" },
            new() { Id = 2, Description = "Other 2" }
        };

        Mock<DbSet<OtherTestEntity>> otherMockDbSet = CreateMockDbSet(otherEntities);
        _mockContext.Setup(c => c.Set<OtherTestEntity>())
            .Returns(otherMockDbSet.Object);

        // Act
        IQueryable<TestEntity> testResult = _basicQueries.GetQueryable<TestEntity>();
        IQueryable<OtherTestEntity> otherResult = _basicQueries.GetQueryable<OtherTestEntity>();

        // Assert
        testResult.ShouldBeSameAs(_mockDbSet.Object);
        otherResult.ShouldBeSameAs(otherMockDbSet.Object);

        _mockContext.Verify(c => c.Set<TestEntity>(), Times.Once);
        _mockContext.Verify(c => c.Set<OtherTestEntity>(), Times.Once);
    }

    [Fact]
    public void GetQueryable_ReturnsQueryableThatSupportsLinq()
    {
        // Act
        IQueryable<TestEntity> result = _basicQueries.GetQueryable<TestEntity>();

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeAssignableTo<IQueryable<TestEntity>>();

        // Test that LINQ operations work on the returned queryable
        var filteredResult = result.Where(e => e.IsActive).ToList();
        filteredResult.Count.ShouldBe(2);
        filteredResult.All(e => e.IsActive).ShouldBeTrue();
    }

    [Fact]
    public void GetQueryable_ReturnsQueryableThatSupportsOrdering()
    {
        // Act
        IQueryable<TestEntity> result = _basicQueries.GetQueryable<TestEntity>();

        // Assert
        var orderedResult = result.OrderBy(e => e.Name).ToList();
        orderedResult.Count.ShouldBe(3);
        orderedResult[0].Name.ShouldBe("Entity 1");
        orderedResult[1].Name.ShouldBe("Entity 2");
        orderedResult[2].Name.ShouldBe("Entity 3");
    }

    [Fact]
    public void GetQueryable_ReturnsQueryableThatSupportsProjection()
    {
        // Act
        IQueryable<TestEntity> result = _basicQueries.GetQueryable<TestEntity>();

        // Assert
        var projectedResult = result.Select(e => e.Name).ToList();
        projectedResult.Count.ShouldBe(3);
        projectedResult.ShouldContain("Entity 1");
        projectedResult.ShouldContain("Entity 2");
        projectedResult.ShouldContain("Entity 3");
    }

    [Fact]
    public void GetQueryable_ReturnsQueryableThatSupportsAggregation()
    {
        // Act
        IQueryable<TestEntity> result = _basicQueries.GetQueryable<TestEntity>();

        // Assert
        int count = result.Count();
        int activeCount = result.Count(e => e.IsActive);
        bool anyInactive = result.Any(e => !e.IsActive);

        count.ShouldBe(3);
        activeCount.ShouldBe(2);
        anyInactive.ShouldBeTrue();
    }

    [Fact]
    public void GetQueryable_ReturnsQueryableThatSupportsComplexQueries()
    {
        // Act
        IQueryable<TestEntity> result = _basicQueries.GetQueryable<TestEntity>();

        // Assert
        var complexResult = result
            .Where(e => e.IsActive)
            .OrderByDescending(e => e.CreatedDate)
            .Select(e => new { e.Id, e.Name })
            .ToList();

        complexResult.Count.ShouldBe(2);
        complexResult[0].Id.ShouldBe(2); // Most recent active entity
        complexResult[1].Id.ShouldBe(1);
    }

    #endregion

    #region Edge Cases and Error Scenarios

    [Fact]
    public void GetQueryable_WithContextThrowingException_PropagatesException()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Context Set access failed");
        _mockContext.Setup(c => c.Set<TestEntity>())
            .Throws(expectedException);

        // Act & Assert
        InvalidOperationException thrownException = Should.Throw<InvalidOperationException>(() =>
            _basicQueries.GetQueryable<TestEntity>());

        thrownException.ShouldBeSameAs(expectedException);
    }

    [Fact]
    public void GetQueryable_WithNullDbSet_ReturnsNull()
    {
        // Arrange
        _mockContext.Setup(c => c.Set<TestEntity>())
            .Returns((DbSet<TestEntity>)null!);

        // Act
        IQueryable<TestEntity> result = _basicQueries.GetQueryable<TestEntity>();

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public void GetQueryable_WithEmptyDbSet_ReturnsEmptyQueryable()
    {
        // Arrange
        var emptyEntities = new List<TestEntity>();
        Mock<DbSet<TestEntity>> emptyMockDbSet = CreateMockDbSet(emptyEntities);
        _mockContext.Setup(c => c.Set<TestEntity>())
            .Returns(emptyMockDbSet.Object);

        // Act
        IQueryable<TestEntity> result = _basicQueries.GetQueryable<TestEntity>();
        var resultList = result.ToList();

        // Assert
        result.ShouldNotBeNull();
        resultList.ShouldBeEmpty();
    }

    #endregion

    #region Performance and Behavior Tests

    [Fact]
    public void GetQueryable_ReturnsSameInstanceFromContext()
    {
        // Act
        IQueryable<TestEntity> result1 = _basicQueries.GetQueryable<TestEntity>();
        IQueryable<TestEntity> result2 = _basicQueries.GetQueryable<TestEntity>();

        // Assert
        result1.ShouldBeSameAs(_mockDbSet.Object);
        result2.ShouldBeSameAs(_mockDbSet.Object);
        result1.ShouldBeSameAs(result2);
    }

    [Fact]
    public void GetQueryable_WithComplexEntityType_WorksCorrectly()
    {
        // Arrange
        var complexEntities = new List<ComplexTestEntity>
        {
            new() { Id = 1, Name = "Complex 1", Category = "A", IsActive = true },
            new() { Id = 2, Name = "Complex 2", Category = "B", IsActive = false },
            new() { Id = 3, Name = "Complex 3", Category = "A", IsActive = true }
        };

        Mock<DbSet<ComplexTestEntity>> complexMockDbSet = CreateMockDbSet(complexEntities);
        _mockContext.Setup(c => c.Set<ComplexTestEntity>())
            .Returns(complexMockDbSet.Object);

        // Act
        IQueryable<ComplexTestEntity> result = _basicQueries.GetQueryable<ComplexTestEntity>();

        // Assert
        result.ShouldNotBeNull();

        var filteredResult = result
            .Where(e => e.Category == "A" && e.IsActive)
            .OrderBy(e => e.Name)
            .ToList();

        filteredResult.Count.ShouldBe(2);
        filteredResult[0].Name.ShouldBe("Complex 1");
        filteredResult[1].Name.ShouldBe("Complex 3");
    }

    [Fact]
    public void GetQueryable_SupportsMultipleSimultaneousQueries()
    {
        // Act
        IQueryable<TestEntity> query1 = _basicQueries.GetQueryable<TestEntity>();
        IQueryable<TestEntity> query2 = _basicQueries.GetQueryable<TestEntity>();

        var result1 = query1.Where(e => e.IsActive).ToList();
        var result2 = query2.Where(e => !e.IsActive).ToList();

        // Assert
        result1.Count.ShouldBe(2);
        result2.Count.ShouldBe(1);
        result1.All(e => e.IsActive).ShouldBeTrue();
        result2.All(e => !e.IsActive).ShouldBeTrue();
    }

    [Fact]
    public void GetQueryable_WithNestedLinqOperations_WorksCorrectly()
    {
        // Act
        IQueryable<TestEntity> result = _basicQueries.GetQueryable<TestEntity>();

        // Assert
        var nestedResult = result
            .Where(e => e.CreatedDate > DateTime.UtcNow.AddDays(-3))
            .GroupBy(e => e.IsActive)
            .Select(g => new { IsActive = g.Key, Count = g.Count() })
            .ToList();

        nestedResult.Count.ShouldBe(2);
        nestedResult.ShouldContain(x => x.IsActive == true && x.Count == 2);
        nestedResult.ShouldContain(x => x.IsActive == false && x.Count == 1);
    }

    #endregion

    #region Type Safety Tests

    [Fact]
    public void GetQueryable_ReturnsCorrectGenericType()
    {
        // Act
        IQueryable<TestEntity> result = _basicQueries.GetQueryable<TestEntity>();

        // Assert
        result.ShouldBeAssignableTo<IQueryable<TestEntity>>();
        result.ElementType.ShouldBe(typeof(TestEntity));
    }

    #endregion

    #region Helper Methods

    // Helper method to create a mock DbSet<T> that works with LINQ methods
    private static Mock<DbSet<T>> CreateMockDbSet<T>(List<T> data) where T : class
    {
        IQueryable<T> queryable = data.AsQueryable();
        var mockSet = new Mock<DbSet<T>>();

        mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
        mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

        return mockSet;
    }

    #endregion
}

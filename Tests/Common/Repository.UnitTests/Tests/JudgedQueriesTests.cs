using Identification.Base;
using Microsoft.EntityFrameworkCore;
using Moq;
using Repository.Core.Contracts;
using Repository.Core.Implementation;
using Repository.UnitTests.Models;
using Shouldly;

namespace Repository.UnitTests.Tests;
public class JudgedQueriesTests
{
    private readonly Mock<DbContext> _mockContext;
    private readonly Mock<IIdentityInfo> _mockIdentityInfo;
    private readonly Mock<IProtected<TestQueryEntity>> _mockProtected;
    private readonly List<IProtected> _protectedEntities;
    private readonly SecureQueryRepo<DbContext> _judgedQueries;
    private readonly Mock<DbSet<TestQueryEntity>> _mockDbSet;
    private readonly List<TestQueryEntity> _testEntities;
    private readonly Guid _testIdentityId = Guid.NewGuid();

    public JudgedQueriesTests()
    {
        // Create test entities for the mock DbSet
        _testEntities =
        [
            new TestQueryEntity { Id = 1, Name = "Entity 1" },
            new TestQueryEntity { Id = 2, Name = "Entity 2" },
            new TestQueryEntity { Id = 3, Name = "Entity 3" }
        ];

        // Set up queryable mock DbSet
        _mockDbSet = CreateMockDbSet(_testEntities);

        // Mock the DbContext
        _mockContext = new Mock<DbContext>();
        _mockContext.Setup(c => c.Set<TestQueryEntity>())
            .Returns(_mockDbSet.Object);

        // Setup identity info mock
        _mockIdentityInfo = new Mock<IIdentityInfo>();

        // Setup protected entity mock
        _mockProtected = new Mock<IProtected<TestQueryEntity>>();
        _mockProtected.Setup(p => p.IsMatch(typeof(TestQueryEntity))).Returns(true);

        // Create a filtered queryable for the mock protected entity
        IQueryable<TestQueryEntity> filteredEntities = _testEntities.Where(e => e.Id == 1).AsQueryable();
        _mockProtected.Setup(p => p.Secured(It.IsAny<Guid>()))
            .Returns(filteredEntities);

        _protectedEntities = [_mockProtected.Object];

        // Create JudgedQueries instance to test
        _judgedQueries = new SecureQueryRepo<DbContext>(
            _mockContext.Object,
            _mockIdentityInfo.Object,
            _protectedEntities);
    }

    #region Secure Method Tests

    [Fact]
    public void Secure_WithMatchingProtection_UsesProtectedEntityFilter()
    {
        // Arrange
        _mockIdentityInfo.Setup(i => i.GetIdentityId()).Returns(_testIdentityId);

        // Act
        IQueryable<TestQueryEntity> result = _judgedQueries.Secure<TestQueryEntity>();
        var resultList = result.ToList();

        // Assert
        resultList.Count.ShouldBe(1);
        resultList[0].Id.ShouldBe(1);
        resultList[0].Name.ShouldBe("Entity 1");

        // Verify the protected entity's Secured method was called with the correct parameters
        _mockProtected.Verify(p => p.Secured(_testIdentityId), Times.Once);

        // Verify context Set method was not called since protection was applied
        _mockContext.Verify(c => c.Set<TestQueryEntity>(), Times.Never);
    }

    [Fact]
    public void Secure_WithNoMatchingProtection_ReturnsEntireDbSet()
    {
        // Arrange
        _mockIdentityInfo.Setup(i => i.GetIdentityId()).Returns(_testIdentityId);

        // Setup a protected entity that doesn't match TestQueryEntity
        var nonMatchingProtected = new Mock<IProtected>();
        nonMatchingProtected.Setup(p => p.IsMatch(typeof(TestQueryEntity))).Returns(false);

        var judgedQueriesWithNonMatchingProtection = new SecureQueryRepo<DbContext>(
            _mockContext.Object,
            _mockIdentityInfo.Object,
            [nonMatchingProtected.Object]);

        // Act
        IQueryable<TestQueryEntity> result = judgedQueriesWithNonMatchingProtection.Secure<TestQueryEntity>();

        // Assert
        result.ShouldBeSameAs(_mockDbSet.Object);

        // Verify the context's Set<T>() method was called
        _mockContext.Verify(c => c.Set<TestQueryEntity>(), Times.Once);
    }

    [Fact]
    public void Secure_WithEmptyProtectionList_ReturnsEntireDbSet()
    {
        // Arrange
        _mockIdentityInfo.Setup(i => i.GetIdentityId()).Returns(_testIdentityId);

        var judgedQueriesWithEmptyProtection = new SecureQueryRepo<DbContext>(
            _mockContext.Object,
            _mockIdentityInfo.Object,
            []);

        // Act
        IQueryable<TestQueryEntity> result = judgedQueriesWithEmptyProtection.Secure<TestQueryEntity>();

        // Assert
        result.ShouldBeSameAs(_mockDbSet.Object);

        // Verify the context's Set<T>() method was called
        _mockContext.Verify(c => c.Set<TestQueryEntity>(), Times.Once);
    }

    [Fact]
    public void Secure_WithMultipleProtectionCandidates_UsesFirstMatch()
    {
        // Arrange
        _mockIdentityInfo.Setup(i => i.GetIdentityId()).Returns(_testIdentityId);

        // Setup a second protected entity that also matches TestQueryEntity but would filter differently
        var mockProtected2 = new Mock<IProtected<TestQueryEntity>>();
        mockProtected2.Setup(p => p.IsMatch(typeof(TestQueryEntity))).Returns(true);

        // This should never be called because it comes second in the list
        IQueryable<TestQueryEntity> alternateFilteredEntities = _testEntities.Where(e => e.Id == 2).AsQueryable();
        mockProtected2.Setup(p => p.Secured(It.IsAny<Guid>()))
            .Returns(alternateFilteredEntities);

        var judgedQueriesWithMultipleProtection = new SecureQueryRepo<DbContext>(
            _mockContext.Object,
            _mockIdentityInfo.Object,
            [_mockProtected.Object, mockProtected2.Object]);

        // Act
        IQueryable<TestQueryEntity> result = judgedQueriesWithMultipleProtection.Secure<TestQueryEntity>();
        var resultList = result.ToList();

        // Assert
        resultList.Count.ShouldBe(1);
        resultList[0].Id.ShouldBe(1);

        // Verify only the first protected entity's Secured method was called
        _mockProtected.Verify(p => p.Secured(_testIdentityId), Times.Once);
        mockProtected2.Verify(p => p.Secured(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public void Secure_WithDifferentEntityType_AppliesCorrectProtection()
    {
        // Arrange
        _mockIdentityInfo.Setup(i => i.GetIdentityId()).Returns(_testIdentityId);

        // Set up a different entity type
        var otherTestEntities = new List<OtherTestEntity>
        {
            new() { Id = 1, Description = "Other 1" },
            new() { Id = 2, Description = "Other 2" }
        };

        Mock<DbSet<OtherTestEntity>> otherMockDbSet = CreateMockDbSet(otherTestEntities);
        _mockContext.Setup(c => c.Set<OtherTestEntity>())
            .Returns(otherMockDbSet.Object);

        // Set up protection for that entity type
        IQueryable<OtherTestEntity> filteredOtherEntities = otherTestEntities.Where(e => e.Id == 1).AsQueryable();
        var otherProtected = new Mock<IProtected<OtherTestEntity>>();
        otherProtected.Setup(p => p.IsMatch(typeof(OtherTestEntity))).Returns(true);
        otherProtected.Setup(p => p.Secured(It.IsAny<Guid>()))
            .Returns(filteredOtherEntities);

        var multiProtectionList = new List<IProtected> { _mockProtected.Object, otherProtected.Object };

        var judgedQueriesWithMultiTypes = new SecureQueryRepo<DbContext>(
            _mockContext.Object,
            _mockIdentityInfo.Object,
            multiProtectionList);

        // Act
        IQueryable<OtherTestEntity> result = judgedQueriesWithMultiTypes.Secure<OtherTestEntity>();
        var resultList = result.ToList();

        // Assert
        resultList.Count.ShouldBe(1);
        resultList[0].Id.ShouldBe(1);
        resultList[0].Description.ShouldBe("Other 1");

        // Verify the other entity's protected.Secured was called
        otherProtected.Verify(p => p.Secured(_testIdentityId), Times.Once);

        // Verify the TestQueryEntity protection was not called
        _mockProtected.Verify(p => p.Secured(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public void Secure_WithMultipleNonMatchingProtections_ReturnsEntireDbSet()
    {
        // Arrange
        _mockIdentityInfo.Setup(i => i.GetIdentityId()).Returns(_testIdentityId);

        // Setup multiple protected entities that don't match TestQueryEntity
        var nonMatchingProtected1 = new Mock<IProtected>();
        nonMatchingProtected1.Setup(p => p.IsMatch(typeof(TestQueryEntity))).Returns(false);

        var nonMatchingProtected2 = new Mock<IProtected>();
        nonMatchingProtected2.Setup(p => p.IsMatch(typeof(TestQueryEntity))).Returns(false);

        var judgedQueriesWithNonMatchingProtections = new SecureQueryRepo<DbContext>(
            _mockContext.Object,
            _mockIdentityInfo.Object,
            [nonMatchingProtected1.Object, nonMatchingProtected2.Object]);

        // Act
        IQueryable<TestQueryEntity> result = judgedQueriesWithNonMatchingProtections.Secure<TestQueryEntity>();

        // Assert
        result.ShouldBeSameAs(_mockDbSet.Object);

        // Verify the context's Set<T>() method was called
        _mockContext.Verify(c => c.Set<TestQueryEntity>(), Times.Once);
    }

    [Fact]
    public void Secure_WithDifferentIdentityIds_PassesCorrectIdentityId()
    {
        // Arrange
        var expectedIdentityId = Guid.NewGuid();
        _mockIdentityInfo.Setup(i => i.GetIdentityId()).Returns(expectedIdentityId);

        // Act
        IQueryable<TestQueryEntity> result = _judgedQueries.Secure<TestQueryEntity>();

        // Assert
        _mockProtected.Verify(p => p.Secured(expectedIdentityId), Times.Once);
    }

    [Fact]
    public void Secure_CalledMultipleTimes_EachCallInvokesProtection()
    {
        // Arrange
        _mockIdentityInfo.Setup(i => i.GetIdentityId()).Returns(_testIdentityId);

        // Act
        IQueryable<TestQueryEntity> result1 = _judgedQueries.Secure<TestQueryEntity>();

        IQueryable<TestQueryEntity> result2 = _judgedQueries.Secure<TestQueryEntity>();

        // Assert
        _mockProtected.Verify(p => p.Secured(_testIdentityId), Times.Exactly(2));
    }

    [Fact]
    public void Secure_WithNullProtectedEntityResult_ReturnsNullQueryable()
    {
        // Arrange
        _mockIdentityInfo.Setup(i => i.GetIdentityId()).Returns(_testIdentityId);
        _mockProtected.Setup(p => p.Secured(It.IsAny<Guid>())).Returns((IQueryable<TestQueryEntity>)null!);

        // Act
        IQueryable<TestQueryEntity> result = _judgedQueries.Secure<TestQueryEntity>();

        // Assert
        result.ShouldBeNull();
        _mockProtected.Verify(p => p.Secured(_testIdentityId), Times.Once);
    }

    [Fact]
    public void Secure_WithEmptyProtectedResult_ReturnsEmptyQueryable()
    {
        // Arrange
        _mockIdentityInfo.Setup(i => i.GetIdentityId()).Returns(_testIdentityId);
        IQueryable<TestQueryEntity> emptyQueryable = new List<TestQueryEntity>().AsQueryable();
        _mockProtected.Setup(p => p.Secured(It.IsAny<Guid>())).Returns(emptyQueryable);

        // Act
        IQueryable<TestQueryEntity> result = _judgedQueries.Secure<TestQueryEntity>();
        var resultList = result.ToList();

        // Assert
        resultList.ShouldBeEmpty();
        _mockProtected.Verify(p => p.Secured(_testIdentityId), Times.Once);
    }

    #endregion

    #region Edge Cases and Error Scenarios

    [Fact]
    public void Secure_WithProtectionThrowingException_PropagatesException()
    {
        // Arrange
        _mockIdentityInfo.Setup(i => i.GetIdentityId()).Returns(_testIdentityId);

        var expectedException = new InvalidOperationException("Protection filter failed");
        _mockProtected.Setup(p => p.Secured(It.IsAny<Guid>()))
            .Throws(expectedException);

        // Act & Assert
        InvalidOperationException thrownException = Should.Throw<InvalidOperationException>(() =>
        {
            IQueryable<TestQueryEntity> result = _judgedQueries.Secure<TestQueryEntity>();
        });

        thrownException.ShouldBeSameAs(expectedException);
    }

    [Fact]
    public void Secure_WithContextThrowingException_PropagatesException()
    {
        // Arrange
        _mockIdentityInfo.Setup(i => i.GetIdentityId()).Returns(_testIdentityId);

        var expectedException = new InvalidOperationException("Context access failed");
        _mockContext.Setup(c => c.Set<TestQueryEntity>())
            .Throws(expectedException);

        var judgedQueriesWithEmptyProtection = new SecureQueryRepo<DbContext>(
            _mockContext.Object,
            _mockIdentityInfo.Object,
            []);

        // Act & Assert
        InvalidOperationException thrownException = Should.Throw<InvalidOperationException>(() =>
            judgedQueriesWithEmptyProtection.Secure<TestQueryEntity>());

        thrownException.ShouldBeSameAs(expectedException);
    }

    [Fact]
    public void Secure_WithIdentityInfoThrowingException_PropagatesException()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Identity access failed");
        _mockIdentityInfo.Setup(i => i.GetIdentityId()).Throws(expectedException);

        // Act & Assert
        InvalidOperationException thrownException = Should.Throw<InvalidOperationException>(() =>
        {
            IQueryable<TestQueryEntity> result = _judgedQueries.Secure<TestQueryEntity>();
        });

        thrownException.ShouldBeSameAs(expectedException);
    }

    #endregion

    #region Performance and Behavior Tests

    [Fact]
    public void Secure_ReturnsQueryable_NotList()
    {
        // Arrange
        _mockIdentityInfo.Setup(i => i.GetIdentityId()).Returns(_testIdentityId);

        // Act
        IQueryable<TestQueryEntity> result = _judgedQueries.Secure<TestQueryEntity>();

        // Assert
        result.ShouldBeOfType<EnumerableQuery<TestQueryEntity>>();
        result.ShouldNotBeNull();
    }

    [Fact]
    public void Secure_WithComplexEntityType_WorksCorrectly()
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

        IQueryable<ComplexTestEntity> filteredComplexEntities = complexEntities.Where(e => e.IsActive).AsQueryable();
        var complexProtected = new Mock<IProtected<ComplexTestEntity>>();
        complexProtected.Setup(p => p.IsMatch(typeof(ComplexTestEntity))).Returns(true);
        complexProtected.Setup(p => p.Secured(It.IsAny<Guid>()))
            .Returns(filteredComplexEntities);

        var judgedQueriesWithComplexType = new SecureQueryRepo<DbContext>(
            _mockContext.Object,
            _mockIdentityInfo.Object,
            [complexProtected.Object]);

        var testIdentityId = Guid.NewGuid();
        _mockIdentityInfo.Setup(i => i.GetIdentityId()).Returns(testIdentityId);

        // Act
        IQueryable<ComplexTestEntity> result = judgedQueriesWithComplexType.Secure<ComplexTestEntity>();
        var resultList = result.ToList();

        // Assert
        resultList.Count.ShouldBe(1);
        resultList[0].Id.ShouldBe(1);
        resultList[0].IsActive.ShouldBeTrue();

        complexProtected.Verify(p => p.Secured(testIdentityId), Times.Once);
    }

    #endregion

    #region Helper Methods and Test Classes

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

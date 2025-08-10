using Microsoft.EntityFrameworkCore;
using Moq;
using Repository.Core.Implementation;
using Repository.UnitTests.Models;
using Shouldly;

namespace Repository.UnitTests.Tests;

public class BasicCommandsTests
{
    private readonly Mock<DbContext> _mockContext;
    private readonly CommandRepo<DbContext> _basicCommands;

    public BasicCommandsTests()
    {
        // Mock the DbContext
        _mockContext = new Mock<DbContext>();

        // Create CommandRepo instance to test
        _basicCommands = new CommandRepo<DbContext>(_mockContext.Object);
    }

    #region Insert Tests

    [Fact]
    public async Task InsertAsync_AddsEntityToContext()
    {
        // Arrange
        var entity = new TestEntity { Id = 1, Name = "Test Entity" };

        // Setup mock expectations
        _mockContext.Setup(c => c.Add(It.IsAny<TestEntity>())).Verifiable();

        // Act
        await _basicCommands.InsertAsync(entity, CancellationToken.None);

        // Assert
        _mockContext.Verify(c => c.Add(entity), Times.Once);
    }

    [Fact]
    public async Task InsertAsync_WithPersistImmediately_SavesChanges()
    {
        // Arrange
        var entity = new TestEntity { Id = 2, Name = "Persisted Entity" };

        // Setup mock expectations
        _mockContext.Setup(c => c.Add(It.IsAny<TestEntity>())).Verifiable();
        _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1)
            .Verifiable();

        // Act
        await _basicCommands.InsertAsync(entity, true, CancellationToken.None);

        // Assert
        _mockContext.Verify(c => c.Add(entity), Times.Once);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task InsertAsync_WithPersistFalse_DoesNotCallSaveChanges()
    {
        // Arrange
        var entity = new TestEntity { Id = 3, Name = "Non-Persisted Entity" };

        // Setup mock expectations
        _mockContext.Setup(c => c.Add(It.IsAny<TestEntity>())).Verifiable();

        // Act
        await _basicCommands.InsertAsync(entity, false, CancellationToken.None);

        // Assert
        _mockContext.Verify(c => c.Add(entity), Times.Once);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task InsertAsync_WithCancellationToken_DoesNotThrow()
    {
        // Arrange
        var entity = new TestEntity { Id = 4, Name = "Token Test Entity" };

        // Setup mock expectations
        _mockContext.Setup(c => c.Add(It.IsAny<TestEntity>())).Verifiable();

        // Act & Assert (should not throw)
        await _basicCommands.InsertAsync(entity, default);

        _mockContext.Verify(c => c.Add(entity), Times.Once);
    }

    [Fact]
    public async Task InsertAsync_WithNullEntity_CallsAddWithNull()
    {
        // Arrange
        TestEntity entity = null!;

        // Setup mock expectations
        _mockContext.Setup(c => c.Add(It.IsAny<TestEntity>())).Verifiable();

        // Act
        await _basicCommands.InsertAsync(entity, CancellationToken.None);

        // Assert
        _mockContext.Verify(c => c.Add(entity), Times.Once);
    }

    #endregion

    #region Update Tests

    [Fact]
    public async Task UpdateAsync_UpdatesEntityInContext()
    {
        // Arrange
        var entity = new TestEntity { Id = 5, Name = "Entity to Update" };

        // Setup mock expectations
        _mockContext.Setup(c => c.Update(It.IsAny<TestEntity>())).Verifiable();

        // Act
        await _basicCommands.UpdateAsync(entity, CancellationToken.None);

        // Assert
        _mockContext.Verify(c => c.Update(entity), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithPersistImmediately_SavesChanges()
    {
        // Arrange
        var entity = new TestEntity { Id = 6, Name = "Persisted Update Entity" };

        // Setup mock expectations
        _mockContext.Setup(c => c.Update(It.IsAny<TestEntity>())).Verifiable();
        _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1)
            .Verifiable();

        // Act
        await _basicCommands.UpdateAsync(entity, true, CancellationToken.None);

        // Assert
        _mockContext.Verify(c => c.Update(entity), Times.Once);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithPersistFalse_DoesNotCallSaveChanges()
    {
        // Arrange
        var entity = new TestEntity { Id = 7, Name = "Non-Persisted Update Entity" };

        // Setup mock expectations
        _mockContext.Setup(c => c.Update(It.IsAny<TestEntity>())).Verifiable();

        // Act
        await _basicCommands.UpdateAsync(entity, false, CancellationToken.None);

        // Assert
        _mockContext.Verify(c => c.Update(entity), Times.Once);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WithDifferentEntityType_WorksCorrectly()
    {
        // Arrange
        var entity = new ComplexTestEntity { Id = 8, Name = "Complex Entity", Category = "Test" };

        // Setup mock expectations
        _mockContext.Setup(c => c.Update(It.IsAny<ComplexTestEntity>())).Verifiable();

        // Act
        await _basicCommands.UpdateAsync(entity, CancellationToken.None);

        // Assert
        _mockContext.Verify(c => c.Update(entity), Times.Once);
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task DeleteAsync_RemovesEntityFromContext()
    {
        // Arrange
        var entity = new TestEntity { Id = 9, Name = "Entity to Delete" };

        // Setup mock expectations
        _mockContext.Setup(c => c.Remove(It.IsAny<TestEntity>())).Verifiable();

        // Act
        await _basicCommands.DeleteAsync(entity, CancellationToken.None);

        // Assert
        _mockContext.Verify(c => c.Remove(entity), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithPersistImmediately_SavesChanges()
    {
        // Arrange
        var entity = new TestEntity { Id = 10, Name = "Persisted Delete Entity" };

        // Setup mock expectations
        _mockContext.Setup(c => c.Remove(It.IsAny<TestEntity>())).Verifiable();
        _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1)
            .Verifiable();

        // Act
        await _basicCommands.DeleteAsync(entity, true, CancellationToken.None);

        // Assert
        _mockContext.Verify(c => c.Remove(entity), Times.Once);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithPersistFalse_DoesNotCallSaveChanges()
    {
        // Arrange
        var entity = new TestEntity { Id = 11, Name = "Non-Persisted Delete Entity" };

        // Setup mock expectations
        _mockContext.Setup(c => c.Remove(It.IsAny<TestEntity>())).Verifiable();

        // Act
        await _basicCommands.DeleteAsync(entity, false, CancellationToken.None);

        // Assert
        _mockContext.Verify(c => c.Remove(entity), Times.Once);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WithDifferentEntityType_WorksCorrectly()
    {
        // Arrange
        var entity = new OtherTestEntity { Id = 12, Description = "Other Entity to Delete" };

        // Setup mock expectations
        _mockContext.Setup(c => c.Remove(It.IsAny<OtherTestEntity>())).Verifiable();

        // Act
        await _basicCommands.DeleteAsync(entity, CancellationToken.None);

        // Assert
        _mockContext.Verify(c => c.Remove(entity), Times.Once);
    }

    #endregion

    #region SaveAsync Tests

    [Fact]
    public async Task SaveAsync_CallsSaveChangesAsyncOnContext()
    {
        // Arrange
        _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1)
            .Verifiable();

        // Act
        await _basicCommands.SaveAsync(CancellationToken.None);

        // Assert
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SaveAsync_WithCancellationToken_PassesTokenToContext()
    {
        // Arrange
        _mockContext.Setup(c => c.SaveChangesAsync(default))
            .ReturnsAsync(1)
            .Verifiable();

        // Act
        await _basicCommands.SaveAsync(default);

        // Assert
        _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task SaveAsync_ReturnsCompletedTask()
    {
        // Arrange
        _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        Task result = _basicCommands.SaveAsync(CancellationToken.None);

        // Assert
        result.IsCompleted.ShouldBeTrue();
        await result; // Ensure it completes without exception
    }

    #endregion

    #region Edge Cases and Error Scenarios

    [Fact]
    public async Task InsertAsync_WithContextThrowingException_PropagatesException()
    {
        // Arrange
        var entity = new TestEntity { Id = 13, Name = "Exception Test Entity" };
        var expectedException = new InvalidOperationException("Context add failed");
        _mockContext.Setup(c => c.Add(It.IsAny<TestEntity>()))
            .Throws(expectedException);

        // Act & Assert
        InvalidOperationException thrownException = await Should.ThrowAsync<InvalidOperationException>(() =>
            _basicCommands.InsertAsync(entity, CancellationToken.None));

        thrownException.ShouldBeSameAs(expectedException);
    }

    [Fact]
    public async Task UpdateAsync_WithContextThrowingException_PropagatesException()
    {
        // Arrange
        var entity = new TestEntity { Id = 14, Name = "Exception Test Entity" };
        var expectedException = new InvalidOperationException("Context update failed");
        _mockContext.Setup(c => c.Update(It.IsAny<TestEntity>()))
            .Throws(expectedException);

        // Act & Assert
        InvalidOperationException thrownException = await Should.ThrowAsync<InvalidOperationException>(() =>
            _basicCommands.UpdateAsync(entity, CancellationToken.None));

        thrownException.ShouldBeSameAs(expectedException);
    }

    [Fact]
    public async Task DeleteAsync_WithContextThrowingException_PropagatesException()
    {
        // Arrange
        var entity = new TestEntity { Id = 15, Name = "Exception Test Entity" };
        var expectedException = new InvalidOperationException("Context remove failed");
        _mockContext.Setup(c => c.Remove(It.IsAny<TestEntity>()))
            .Throws(expectedException);

        // Act & Assert
        InvalidOperationException thrownException = await Should.ThrowAsync<InvalidOperationException>(() =>
            _basicCommands.DeleteAsync(entity, CancellationToken.None));

        thrownException.ShouldBeSameAs(expectedException);
    }

    [Fact]
    public async Task SaveAsync_WithContextThrowingException_PropagatesException()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Save failed");
        _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        InvalidOperationException thrownException = await Should.ThrowAsync<InvalidOperationException>(() =>
            _basicCommands.SaveAsync(CancellationToken.None));

        thrownException.ShouldBeSameAs(expectedException);
    }

    [Fact]
    public async Task InsertAsync_WithCancelledToken_ThrowsOperationCancelledException()
    {
        // Arrange
        var entity = new TestEntity { Id = 16, Name = "Cancelled Token Test Entity" };
        _mockContext.Setup(c => c.Add(It.IsAny<TestEntity>()))
            .Throws(new OperationCanceledException());

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(() =>
            _basicCommands.InsertAsync(entity, default));
    }

    [Fact]
    public async Task SaveAsync_WithCancelledToken_ThrowsOperationCancelledException()
    {
        // Arrange
        _mockContext.Setup(c => c.SaveChangesAsync(default))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(() =>
            _basicCommands.SaveAsync(default));
    }

    #endregion

    #region Multiple Operations Tests

    [Fact]
    public async Task MultipleOperations_WithoutPersist_DoesNotCallSaveChanges()
    {
        // Arrange
        var entity1 = new TestEntity { Id = 17, Name = "Entity 1" };
        var entity2 = new TestEntity { Id = 18, Name = "Entity 2" };
        var entity3 = new TestEntity { Id = 19, Name = "Entity 3" };

        _mockContext.Setup(c => c.Add(It.IsAny<TestEntity>())).Verifiable();
        _mockContext.Setup(c => c.Update(It.IsAny<TestEntity>())).Verifiable();
        _mockContext.Setup(c => c.Remove(It.IsAny<TestEntity>())).Verifiable();

        // Act
        await _basicCommands.InsertAsync(entity1, CancellationToken.None);
        await _basicCommands.UpdateAsync(entity2, CancellationToken.None);
        await _basicCommands.DeleteAsync(entity3, CancellationToken.None);

        // Assert
        _mockContext.Verify(c => c.Add(entity1), Times.Once);
        _mockContext.Verify(c => c.Update(entity2), Times.Once);
        _mockContext.Verify(c => c.Remove(entity3), Times.Once);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task MultipleOperations_ThenSave_CallsSaveChangesOnce()
    {
        // Arrange
        var entity1 = new TestEntity { Id = 20, Name = "Entity 1" };
        var entity2 = new TestEntity { Id = 21, Name = "Entity 2" };

        _mockContext.Setup(c => c.Add(It.IsAny<TestEntity>())).Verifiable();
        _mockContext.Setup(c => c.Update(It.IsAny<TestEntity>())).Verifiable();
        _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(2)
            .Verifiable();

        // Act
        await _basicCommands.InsertAsync(entity1, CancellationToken.None);
        await _basicCommands.UpdateAsync(entity2, CancellationToken.None);
        await _basicCommands.SaveAsync(CancellationToken.None);

        // Assert
        _mockContext.Verify(c => c.Add(entity1), Times.Once);
        _mockContext.Verify(c => c.Update(entity2), Times.Once);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Performance and Behavior Tests

    [Fact]
    public async Task Operations_ReturnsTaskCompletedTask()
    {
        // Arrange
        var entity = new TestEntity { Id = 22, Name = "Task Test Entity" };

        // Act
        Task insertTask = _basicCommands.InsertAsync(entity, CancellationToken.None);
        Task updateTask = _basicCommands.UpdateAsync(entity, CancellationToken.None);
        Task deleteTask = _basicCommands.DeleteAsync(entity, CancellationToken.None);

        // Assert
        insertTask.IsCompleted.ShouldBeTrue();
        updateTask.IsCompleted.ShouldBeTrue();
        deleteTask.IsCompleted.ShouldBeTrue();

        await insertTask;
        await updateTask;
        await deleteTask;
    }

    [Fact]
    public async Task Operations_WithComplexEntityType_WorksCorrectly()
    {
        // Arrange
        var complexEntity = new ComplexTestEntity
        {
            Id = 23,
            Name = "Complex Entity",
            Category = "Test Category",
            IsActive = true
        };

        _mockContext.Setup(c => c.Add(It.IsAny<ComplexTestEntity>())).Verifiable();
        _mockContext.Setup(c => c.Update(It.IsAny<ComplexTestEntity>())).Verifiable();
        _mockContext.Setup(c => c.Remove(It.IsAny<ComplexTestEntity>())).Verifiable();

        // Act
        await _basicCommands.InsertAsync(complexEntity, CancellationToken.None);
        await _basicCommands.UpdateAsync(complexEntity, CancellationToken.None);
        await _basicCommands.DeleteAsync(complexEntity, CancellationToken.None);

        // Assert
        _mockContext.Verify(c => c.Add(complexEntity), Times.Once);
        _mockContext.Verify(c => c.Update(complexEntity), Times.Once);
        _mockContext.Verify(c => c.Remove(complexEntity), Times.Once);
    }

    #endregion
}

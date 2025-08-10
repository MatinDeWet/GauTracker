using Identification.Base;
using Microsoft.EntityFrameworkCore;
using Moq;
using Repository.Core.Contracts;
using Repository.Core.Enums;
using Repository.Core.Implementation;
using Repository.UnitTests.Models;
using Shouldly;

namespace Repository.UnitTests.Tests;
public class JudgedCommandsTests
{
    private readonly Mock<DbContext> _mockContext;
    private readonly Mock<IIdentityInfo> _mockIdentityInfo;
    private readonly Mock<IProtected<TestCommandEntity>> _mockProtected;
    private readonly List<IProtected> _protectedEntities;
    private readonly SecureCommandRepo<DbContext> _judgedCommands;
    private readonly Guid _testIdentityId = Guid.NewGuid();

    public JudgedCommandsTests()
    {
        // Mock the DbContext
        _mockContext = new Mock<DbContext>();

        // Setup identity info mock
        _mockIdentityInfo = new Mock<IIdentityInfo>();

        // Setup protected entity mock
        _mockProtected = new Mock<IProtected<TestCommandEntity>>();
        _mockProtected.Setup(p => p.IsMatch(typeof(TestCommandEntity))).Returns(true);

        _protectedEntities = [_mockProtected.Object];

        // Create JudgedCommands instance to test
        _judgedCommands = new SecureCommandRepo<DbContext>(
            _mockContext.Object,
            _mockIdentityInfo.Object,
            _protectedEntities);
    }

    #region Insert Tests

    [Fact]
    public async Task InsertAsync_WithAccessGranted_AddsEntityToContext()
    {
        // Arrange
        _mockIdentityInfo.Setup(i => i.GetIdentityId()).Returns(_testIdentityId);

        _mockProtected
            .Setup(p => p.HasAccess(
                It.IsAny<TestCommandEntity>(),
                It.IsAny<Guid>(),
                It.IsAny<RepositoryOperationEnum>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var entity = new TestCommandEntity { Id = 1, Name = "Test Entity" };

        // Setup mock expectations
        _mockContext.Setup(c => c.Add(It.IsAny<TestCommandEntity>())).Verifiable();

        // Act
        await _judgedCommands.InsertAsync(entity, CancellationToken.None);

        // Assert
        _mockContext.Verify(c => c.Add(entity), Times.Once);

        // Verify HasAccess was called with the correct parameters
        _mockProtected.Verify(p => p.HasAccess(
            entity,
            _testIdentityId,
            RepositoryOperationEnum.Insert,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task InsertAsync_WithPersistImmediately_SavesChanges()
    {
        // Arrange
        _mockIdentityInfo.Setup(i => i.GetIdentityId()).Returns(_testIdentityId);

        _mockProtected
            .Setup(p => p.HasAccess(
                It.IsAny<TestCommandEntity>(),
                It.IsAny<Guid>(),
                It.IsAny<RepositoryOperationEnum>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var entity = new TestCommandEntity { Id = 2, Name = "Persisted Entity" };

        // Setup mock expectations
        _mockContext.Setup(c => c.Add(It.IsAny<TestCommandEntity>())).Verifiable();
        _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1)
            .Verifiable();

        // Act
        await _judgedCommands.InsertAsync(entity, true, CancellationToken.None);

        // Assert
        _mockContext.Verify(c => c.Add(entity), Times.Once);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task InsertAsync_WithAccessDenied_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        _mockIdentityInfo.Setup(i => i.GetIdentityId()).Returns(_testIdentityId);

        _mockProtected
            .Setup(p => p.HasAccess(
                It.IsAny<TestCommandEntity>(),
                It.IsAny<Guid>(),
                It.IsAny<RepositoryOperationEnum>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var entity = new TestCommandEntity { Id = 3, Name = "Unauthorized Entity" };

        // Act & Assert
        await Should.ThrowAsync<UnauthorizedAccessException>(() =>
            _judgedCommands.InsertAsync(entity, CancellationToken.None));

        // Verify Add was never called
        _mockContext.Verify(c => c.Add(It.IsAny<TestCommandEntity>()), Times.Never);
    }

    [Fact]
    public async Task InsertAsync_WithNoProtectionForEntity_AddsEntityToContext()
    {
        // Arrange
        _mockIdentityInfo.Setup(i => i.GetIdentityId()).Returns(_testIdentityId);

        // Create JudgedCommands with empty protections list
        var judgedCommandsWithoutProtection = new SecureCommandRepo<DbContext>(
            _mockContext.Object,
            _mockIdentityInfo.Object,
            []);

        var entity = new TestCommandEntity { Id = 4, Name = "Unprotected Entity" };

        // Setup mock expectations
        _mockContext.Setup(c => c.Add(It.IsAny<TestCommandEntity>())).Verifiable();

        // Act
        await judgedCommandsWithoutProtection.InsertAsync(entity, CancellationToken.None);

        // Assert
        _mockContext.Verify(c => c.Add(entity), Times.Once);
    }

    [Fact]
    public async Task InsertAsync_WithPersistFalse_DoesNotCallSaveChanges()
    {
        // Arrange
        _mockIdentityInfo.Setup(i => i.GetIdentityId()).Returns(_testIdentityId);

        _mockProtected
            .Setup(p => p.HasAccess(
                It.IsAny<TestCommandEntity>(),
                It.IsAny<Guid>(),
                It.IsAny<RepositoryOperationEnum>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var entity = new TestCommandEntity { Id = 5, Name = "Non-Persisted Entity" };

        // Setup mock expectations
        _mockContext.Setup(c => c.Add(It.IsAny<TestCommandEntity>())).Verifiable();

        // Act
        await _judgedCommands.InsertAsync(entity, false, CancellationToken.None);

        // Assert
        _mockContext.Verify(c => c.Add(entity), Times.Once);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task InsertAsync_WithCancellationToken_PassesTokenToHasAccess()
    {
        // Arrange
        _mockIdentityInfo.Setup(i => i.GetIdentityId()).Returns(_testIdentityId);

        _mockProtected
            .Setup(p => p.HasAccess(
                It.IsAny<TestCommandEntity>(),
                It.IsAny<Guid>(),
                It.IsAny<RepositoryOperationEnum>(),
                default))
            .ReturnsAsync(true);

        var entity = new TestCommandEntity { Id = 6, Name = "Token Test Entity" };

        // Act
        await _judgedCommands.InsertAsync(entity, default);

        // Assert
        _mockProtected.Verify(p => p.HasAccess(
            entity,
            _testIdentityId,
            RepositoryOperationEnum.Insert,
            default), Times.Once);
    }

    #endregion

    #region Update Tests

    [Fact]
    public async Task UpdateAsync_WithAccessGranted_UpdatesEntityInContext()
    {
        // Arrange
        _mockIdentityInfo.Setup(i => i.GetIdentityId()).Returns(_testIdentityId);

        _mockProtected
            .Setup(p => p.HasAccess(
                It.IsAny<TestCommandEntity>(),
                It.IsAny<Guid>(),
                It.IsAny<RepositoryOperationEnum>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var entity = new TestCommandEntity { Id = 7, Name = "Entity to Update" };

        // Setup mock expectations
        _mockContext.Setup(c => c.Update(It.IsAny<TestCommandEntity>())).Verifiable();

        // Act
        await _judgedCommands.UpdateAsync(entity, CancellationToken.None);

        // Assert
        _mockContext.Verify(c => c.Update(entity), Times.Once);

        // Verify HasAccess was called with the correct parameters
        _mockProtected.Verify(p => p.HasAccess(
            entity,
            _testIdentityId,
            RepositoryOperationEnum.Update,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithPersistImmediately_SavesChanges()
    {
        // Arrange
        _mockIdentityInfo.Setup(i => i.GetIdentityId()).Returns(_testIdentityId);

        _mockProtected
            .Setup(p => p.HasAccess(
                It.IsAny<TestCommandEntity>(),
                It.IsAny<Guid>(),
                It.IsAny<RepositoryOperationEnum>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var entity = new TestCommandEntity { Id = 8, Name = "Persisted Update Entity" };

        // Setup mock expectations
        _mockContext.Setup(c => c.Update(It.IsAny<TestCommandEntity>())).Verifiable();
        _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1)
            .Verifiable();

        // Act
        await _judgedCommands.UpdateAsync(entity, true, CancellationToken.None);

        // Assert
        _mockContext.Verify(c => c.Update(entity), Times.Once);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithAccessDenied_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        _mockIdentityInfo.Setup(i => i.GetIdentityId()).Returns(_testIdentityId);

        _mockProtected
            .Setup(p => p.HasAccess(
                It.IsAny<TestCommandEntity>(),
                It.IsAny<Guid>(),
                It.IsAny<RepositoryOperationEnum>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var entity = new TestCommandEntity { Id = 9, Name = "Unauthorized Update Entity" };

        // Act & Assert
        await Should.ThrowAsync<UnauthorizedAccessException>(() =>
            _judgedCommands.UpdateAsync(entity, CancellationToken.None));

        // Verify Update was never called
        _mockContext.Verify(c => c.Update(It.IsAny<TestCommandEntity>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WithPersistFalse_DoesNotCallSaveChanges()
    {
        // Arrange
        _mockIdentityInfo.Setup(i => i.GetIdentityId()).Returns(_testIdentityId);

        _mockProtected
            .Setup(p => p.HasAccess(
                It.IsAny<TestCommandEntity>(),
                It.IsAny<Guid>(),
                It.IsAny<RepositoryOperationEnum>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var entity = new TestCommandEntity { Id = 10, Name = "Non-Persisted Update Entity" };

        // Setup mock expectations
        _mockContext.Setup(c => c.Update(It.IsAny<TestCommandEntity>())).Verifiable();

        // Act
        await _judgedCommands.UpdateAsync(entity, false, CancellationToken.None);

        // Assert
        _mockContext.Verify(c => c.Update(entity), Times.Once);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task DeleteAsync_WithAccessGranted_RemovesEntityFromContext()
    {
        // Arrange
        _mockIdentityInfo.Setup(i => i.GetIdentityId()).Returns(_testIdentityId);

        _mockProtected
            .Setup(p => p.HasAccess(
                It.IsAny<TestCommandEntity>(),
                It.IsAny<Guid>(),
                It.IsAny<RepositoryOperationEnum>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var entity = new TestCommandEntity { Id = 11, Name = "Entity to Delete" };

        // Setup mock expectations
        _mockContext.Setup(c => c.Remove(It.IsAny<TestCommandEntity>())).Verifiable();

        // Act
        await _judgedCommands.DeleteAsync(entity, CancellationToken.None);

        // Assert
        _mockContext.Verify(c => c.Remove(entity), Times.Once);

        // Verify HasAccess was called with the correct parameters
        _mockProtected.Verify(p => p.HasAccess(
            entity,
            _testIdentityId,
            RepositoryOperationEnum.Delete,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithPersistImmediately_SavesChanges()
    {
        // Arrange
        _mockIdentityInfo.Setup(i => i.GetIdentityId()).Returns(_testIdentityId);

        _mockProtected
            .Setup(p => p.HasAccess(
                It.IsAny<TestCommandEntity>(),
                It.IsAny<Guid>(),
                It.IsAny<RepositoryOperationEnum>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var entity = new TestCommandEntity { Id = 12, Name = "Persisted Delete Entity" };

        // Setup mock expectations
        _mockContext.Setup(c => c.Remove(It.IsAny<TestCommandEntity>())).Verifiable();
        _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1)
            .Verifiable();

        // Act
        await _judgedCommands.DeleteAsync(entity, true, CancellationToken.None);

        // Assert
        _mockContext.Verify(c => c.Remove(entity), Times.Once);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithAccessDenied_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        _mockIdentityInfo.Setup(i => i.GetIdentityId()).Returns(_testIdentityId);

        _mockProtected
            .Setup(p => p.HasAccess(
                It.IsAny<TestCommandEntity>(),
                It.IsAny<Guid>(),
                It.IsAny<RepositoryOperationEnum>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var entity = new TestCommandEntity { Id = 13, Name = "Unauthorized Delete Entity" };

        // Act & Assert
        await Should.ThrowAsync<UnauthorizedAccessException>(() =>
            _judgedCommands.DeleteAsync(entity, CancellationToken.None));

        // Verify Remove was never called
        _mockContext.Verify(c => c.Remove(It.IsAny<TestCommandEntity>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WithPersistFalse_DoesNotCallSaveChanges()
    {
        // Arrange
        _mockIdentityInfo.Setup(i => i.GetIdentityId()).Returns(_testIdentityId);

        _mockProtected
            .Setup(p => p.HasAccess(
                It.IsAny<TestCommandEntity>(),
                It.IsAny<Guid>(),
                It.IsAny<RepositoryOperationEnum>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var entity = new TestCommandEntity { Id = 14, Name = "Non-Persisted Delete Entity" };

        // Setup mock expectations
        _mockContext.Setup(c => c.Remove(It.IsAny<TestCommandEntity>())).Verifiable();

        // Act
        await _judgedCommands.DeleteAsync(entity, false, CancellationToken.None);

        // Assert
        _mockContext.Verify(c => c.Remove(entity), Times.Once);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
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
        await _judgedCommands.SaveAsync(CancellationToken.None);

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
        await _judgedCommands.SaveAsync(default);

        // Assert
        _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
    }

    #endregion

    #region HasAccess (Private Method) Tests via Public Methods

    [Fact]
    public async Task HasAccess_WithNonMatchingProtection_ReturnsTrue()
    {
        // Arrange
        _mockIdentityInfo.Setup(i => i.GetIdentityId()).Returns(_testIdentityId);

        // Setup a protected entity that doesn't match TestCommandEntity
        var nonMatchingProtected = new Mock<IProtected>();
        nonMatchingProtected.Setup(p => p.IsMatch(typeof(TestCommandEntity))).Returns(false);

        var judgedCommandsWithNonMatchingProtection = new SecureCommandRepo<DbContext>(
            _mockContext.Object,
            _mockIdentityInfo.Object,
            [nonMatchingProtected.Object]);

        var entity = new TestCommandEntity { Id = 15, Name = "Non-Matching Protection Entity" };

        // Setup mock expectations
        _mockContext.Setup(c => c.Add(It.IsAny<TestCommandEntity>())).Verifiable();

        // Act
        await judgedCommandsWithNonMatchingProtection.InsertAsync(entity, CancellationToken.None);

        // Assert
        _mockContext.Verify(c => c.Add(entity), Times.Once);
    }

    [Fact]
    public async Task HasAccess_WithMultipleProtectionCandidates_UsesFirstMatch()
    {
        // Arrange
        _mockIdentityInfo.Setup(i => i.GetIdentityId()).Returns(_testIdentityId);

        // First protection will grant access
        _mockProtected
            .Setup(p => p.HasAccess(
                It.IsAny<TestCommandEntity>(),
                It.IsAny<Guid>(),
                It.IsAny<RepositoryOperationEnum>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Second protection would deny access, but should never be called
        var mockProtected2 = new Mock<IProtected<TestCommandEntity>>();
        mockProtected2.Setup(p => p.IsMatch(typeof(TestCommandEntity))).Returns(true);
        mockProtected2
            .Setup(p => p.HasAccess(
                It.IsAny<TestCommandEntity>(),
                It.IsAny<Guid>(),
                It.IsAny<RepositoryOperationEnum>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var judgedCommandsWithMultipleProtection = new SecureCommandRepo<DbContext>(
            _mockContext.Object,
            _mockIdentityInfo.Object,
            [_mockProtected.Object, mockProtected2.Object]);

        var entity = new TestCommandEntity { Id = 16, Name = "Multiple Protection Entity" };

        // Setup mock expectations
        _mockContext.Setup(c => c.Add(It.IsAny<TestCommandEntity>())).Verifiable();

        // Act
        await judgedCommandsWithMultipleProtection.InsertAsync(entity, CancellationToken.None);

        // Assert
        _mockContext.Verify(c => c.Add(entity), Times.Once);

        // Verify only the first protected entity's HasAccess method was called
        _mockProtected.Verify(p => p.HasAccess(
            entity,
            _testIdentityId,
            RepositoryOperationEnum.Insert,
            It.IsAny<CancellationToken>()), Times.Once);

        mockProtected2.Verify(p => p.HasAccess(
            It.IsAny<TestCommandEntity>(),
            It.IsAny<Guid>(),
            It.IsAny<RepositoryOperationEnum>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HasAccess_WithMultipleNonMatchingProtections_ReturnsTrue()
    {
        // Arrange
        _mockIdentityInfo.Setup(i => i.GetIdentityId()).Returns(_testIdentityId);

        // Setup multiple protected entities that don't match TestCommandEntity
        var nonMatchingProtected1 = new Mock<IProtected>();
        nonMatchingProtected1.Setup(p => p.IsMatch(typeof(TestCommandEntity))).Returns(false);

        var nonMatchingProtected2 = new Mock<IProtected>();
        nonMatchingProtected2.Setup(p => p.IsMatch(typeof(TestCommandEntity))).Returns(false);

        var judgedCommandsWithNonMatchingProtections = new SecureCommandRepo<DbContext>(
            _mockContext.Object,
            _mockIdentityInfo.Object,
            [nonMatchingProtected1.Object, nonMatchingProtected2.Object]);

        var entity = new TestCommandEntity { Id = 17, Name = "Multiple Non-Matching Protections Entity" };

        // Setup mock expectations
        _mockContext.Setup(c => c.Add(It.IsAny<TestCommandEntity>())).Verifiable();

        // Act
        await judgedCommandsWithNonMatchingProtections.InsertAsync(entity, CancellationToken.None);

        // Assert
        _mockContext.Verify(c => c.Add(entity), Times.Once);
    }

    [Fact]
    public async Task HasAccess_WithNullEntity_StillCallsProtection()
    {
        // Arrange
        _mockIdentityInfo.Setup(i => i.GetIdentityId()).Returns(_testIdentityId);

        _mockProtected
            .Setup(p => p.HasAccess(
                null!,
                It.IsAny<Guid>(),
                It.IsAny<RepositoryOperationEnum>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await _judgedCommands.InsertAsync<TestCommandEntity>(null!, CancellationToken.None);

        // Assert
        _mockProtected.Verify(p => p.HasAccess(
            null!,
            _testIdentityId,
            RepositoryOperationEnum.Insert,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Edge Cases and Error Scenarios

    [Fact]
    public async Task InsertAsync_WithProtectionThrowingException_PropagatesException()
    {
        // Arrange
        _mockIdentityInfo.Setup(i => i.GetIdentityId()).Returns(_testIdentityId);

        var expectedException = new InvalidOperationException("Protection check failed");
        _mockProtected
            .Setup(p => p.HasAccess(
                It.IsAny<TestCommandEntity>(),
                It.IsAny<Guid>(),
                It.IsAny<RepositoryOperationEnum>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        var entity = new TestCommandEntity { Id = 18, Name = "Exception Test Entity" };

        // Act & Assert
        InvalidOperationException thrownException = await Should.ThrowAsync<InvalidOperationException>(() =>
            _judgedCommands.InsertAsync(entity, CancellationToken.None));

        thrownException.ShouldBeSameAs(expectedException);
        _mockContext.Verify(c => c.Add(It.IsAny<TestCommandEntity>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WithProtectionThrowingException_PropagatesException()
    {
        // Arrange
        _mockIdentityInfo.Setup(i => i.GetIdentityId()).Returns(_testIdentityId);

        var expectedException = new InvalidOperationException("Protection check failed");
        _mockProtected
            .Setup(p => p.HasAccess(
                It.IsAny<TestCommandEntity>(),
                It.IsAny<Guid>(),
                It.IsAny<RepositoryOperationEnum>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        var entity = new TestCommandEntity { Id = 19, Name = "Exception Test Entity" };

        // Act & Assert
        InvalidOperationException thrownException = await Should.ThrowAsync<InvalidOperationException>(() =>
            _judgedCommands.UpdateAsync(entity, CancellationToken.None));

        thrownException.ShouldBeSameAs(expectedException);
        _mockContext.Verify(c => c.Update(It.IsAny<TestCommandEntity>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WithProtectionThrowingException_PropagatesException()
    {
        // Arrange
        _mockIdentityInfo.Setup(i => i.GetIdentityId()).Returns(_testIdentityId);

        var expectedException = new InvalidOperationException("Protection check failed");
        _mockProtected
            .Setup(p => p.HasAccess(
                It.IsAny<TestCommandEntity>(),
                It.IsAny<Guid>(),
                It.IsAny<RepositoryOperationEnum>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        var entity = new TestCommandEntity { Id = 20, Name = "Exception Test Entity" };

        // Act & Assert
        InvalidOperationException thrownException = await Should.ThrowAsync<InvalidOperationException>(() =>
            _judgedCommands.DeleteAsync(entity, CancellationToken.None));

        thrownException.ShouldBeSameAs(expectedException);
        _mockContext.Verify(c => c.Remove(It.IsAny<TestCommandEntity>()), Times.Never);
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
            _judgedCommands.SaveAsync(CancellationToken.None));

        thrownException.ShouldBeSameAs(expectedException);
    }

    [Fact]
    public async Task InsertAsync_WithCancelledToken_ThrowsOperationCancelledException()
    {
        // Arrange
        _mockIdentityInfo.Setup(i => i.GetIdentityId()).Returns(_testIdentityId);

        _mockProtected
            .Setup(p => p.HasAccess(
                It.IsAny<TestCommandEntity>(),
                It.IsAny<Guid>(),
                It.IsAny<RepositoryOperationEnum>(),
                default))
            .ThrowsAsync(new OperationCanceledException());

        var entity = new TestCommandEntity { Id = 21, Name = "Cancelled Token Test Entity" };

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(() =>
            _judgedCommands.InsertAsync(entity, default));
    }

    #endregion
}

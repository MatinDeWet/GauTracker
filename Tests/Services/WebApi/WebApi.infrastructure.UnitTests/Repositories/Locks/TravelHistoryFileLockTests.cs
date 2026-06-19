using Microsoft.EntityFrameworkCore;
using MockQueryable.NSubstitute;
using NSubstitute;
using Repository.Enums;
using Shouldly;
using Shared.Domain.Entities;
using Shared.Persistence.Data.Contexts;
using WebApi.infrastructure.Repositories.Locks;
using WebApi.infrastructure.UnitTests.TestDoubles;
using Xunit;

namespace WebApi.infrastructure.UnitTests.Repositories.Locks;

public class TravelHistoryFileLockTests
{
    private readonly GauContext _context = Substitute.For<GauContext>();

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Secured_ReturnsOnlyFilesWhoseCardIsOwnedByTheGivenUser()
    {
        List<TravelHistoryFile> data =
        [
            TestTravelHistoryFile.ForCard(cardId: 10, userId: 1, id: 1),
            TestTravelHistoryFile.ForCard(cardId: 20, userId: 2, id: 2),
            TestTravelHistoryFile.ForCard(cardId: 30, userId: 1, id: 3),
        ];
        DbSet<TravelHistoryFile> set = data.BuildMockDbSet();
        _context.Set<TravelHistoryFile>().Returns(set);

        TravelHistoryFileLock sut = new(_context);

        List<TravelHistoryFile> result = await sut.Secured(1).ToListAsync(Ct);

        result.Select(x => x.Id).ShouldBe([1, 3], ignoreOrder: true);
    }

    [Fact]
    public async Task Secured_ReturnsEmpty_WhenTheUserOwnsNoCards()
    {
        List<TravelHistoryFile> data =
        [
            TestTravelHistoryFile.ForCard(cardId: 10, userId: 1, id: 1),
            TestTravelHistoryFile.ForCard(cardId: 20, userId: 2, id: 2),
        ];
        DbSet<TravelHistoryFile> set = data.BuildMockDbSet();
        _context.Set<TravelHistoryFile>().Returns(set);

        TravelHistoryFileLock sut = new(_context);

        List<TravelHistoryFile> result = await sut.Secured(99).ToListAsync(Ct);

        result.ShouldBeEmpty();
    }

    [Theory]
    [InlineData(RepositoryOperationEnum.Read)]
    [InlineData(RepositoryOperationEnum.Insert)]
    [InlineData(RepositoryOperationEnum.Update)]
    [InlineData(RepositoryOperationEnum.Delete)]
    public async Task HasAccess_IsTrue_WhenTheUserOwnsTheFilesCard(RepositoryOperationEnum operation)
    {
        List<Card> cards = [TestCard.OwnedBy(userId: 5, id: 10)];
        DbSet<Card> set = cards.BuildMockDbSet();
        _context.Set<Card>().Returns(set);

        TravelHistoryFileLock sut = new(_context);
        TravelHistoryFile file = TestTravelHistoryFile.ForCard(cardId: 10, userId: 5);

        bool result = await sut.HasAccess(file, userId: 5, operation, Ct);

        result.ShouldBeTrue();
    }

    [Theory]
    [InlineData(RepositoryOperationEnum.Read)]
    [InlineData(RepositoryOperationEnum.Insert)]
    [InlineData(RepositoryOperationEnum.Update)]
    [InlineData(RepositoryOperationEnum.Delete)]
    public async Task HasAccess_IsFalse_WhenTheUserDoesNotOwnTheFilesCard(RepositoryOperationEnum operation)
    {
        List<Card> cards = [TestCard.OwnedBy(userId: 5, id: 10)];
        DbSet<Card> set = cards.BuildMockDbSet();
        _context.Set<Card>().Returns(set);

        TravelHistoryFileLock sut = new(_context);
        TravelHistoryFile file = TestTravelHistoryFile.ForCard(cardId: 10, userId: 5);

        bool result = await sut.HasAccess(file, userId: 6, operation, Ct);

        result.ShouldBeFalse();
    }

    [Fact]
    public void IsMatch_IsTrue_ForTravelHistoryFile()
    {
        TravelHistoryFileLock sut = new(_context);

        sut.IsMatch(typeof(TravelHistoryFile)).ShouldBeTrue();
    }

    [Fact]
    public void IsMatch_IsFalse_ForUnrelatedType()
    {
        TravelHistoryFileLock sut = new(_context);

        sut.IsMatch(typeof(string)).ShouldBeFalse();
    }
}

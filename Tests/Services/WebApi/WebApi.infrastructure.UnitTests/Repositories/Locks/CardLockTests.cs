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

public class CardLockTests
{
    private readonly GauContext _context = Substitute.For<GauContext>();

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Secured_ReturnsOnlyTheCardsOwnedByTheGivenUser()
    {
        List<Card> data =
        [
            TestCard.OwnedBy(userId: 1, id: 1),
            TestCard.OwnedBy(userId: 2, id: 2),
            TestCard.OwnedBy(userId: 1, id: 3),
        ];
        DbSet<Card> set = data.BuildMockDbSet();
        _context.Set<Card>().Returns(set);

        CardLock sut = new(_context);

        List<Card> result = await sut.Secured(1).ToListAsync(Ct);

        result.Select(x => x.Id).ShouldBe([1, 3], ignoreOrder: true);
    }

    [Fact]
    public async Task Secured_ReturnsEmpty_WhenTheUserOwnsNoCards()
    {
        List<Card> data = [TestCard.OwnedBy(userId: 1, id: 1), TestCard.OwnedBy(userId: 2, id: 2)];
        DbSet<Card> set = data.BuildMockDbSet();
        _context.Set<Card>().Returns(set);

        CardLock sut = new(_context);

        List<Card> result = await sut.Secured(99).ToListAsync(Ct);

        result.ShouldBeEmpty();
    }

    [Theory]
    [InlineData(RepositoryOperationEnum.Read)]
    [InlineData(RepositoryOperationEnum.Insert)]
    [InlineData(RepositoryOperationEnum.Update)]
    [InlineData(RepositoryOperationEnum.Delete)]
    public async Task HasAccess_IsTrue_WhenTheUserOwnsTheCard(RepositoryOperationEnum operation)
    {
        CardLock sut = new(_context);
        Card card = TestCard.OwnedBy(userId: 5);

        bool result = await sut.HasAccess(card, userId: 5, operation, Ct);

        result.ShouldBeTrue();
    }

    [Theory]
    [InlineData(RepositoryOperationEnum.Read)]
    [InlineData(RepositoryOperationEnum.Insert)]
    [InlineData(RepositoryOperationEnum.Update)]
    [InlineData(RepositoryOperationEnum.Delete)]
    public async Task HasAccess_IsFalse_WhenTheUserDoesNotOwnTheCard(RepositoryOperationEnum operation)
    {
        CardLock sut = new(_context);
        Card card = TestCard.OwnedBy(userId: 5);

        bool result = await sut.HasAccess(card, userId: 6, operation, Ct);

        result.ShouldBeFalse();
    }

    [Fact]
    public void IsMatch_IsTrue_ForCard()
    {
        CardLock sut = new(_context);

        sut.IsMatch(typeof(Card)).ShouldBeTrue();
    }

    [Fact]
    public void IsMatch_IsFalse_ForUnrelatedType()
    {
        CardLock sut = new(_context);

        sut.IsMatch(typeof(string)).ShouldBeFalse();
    }
}

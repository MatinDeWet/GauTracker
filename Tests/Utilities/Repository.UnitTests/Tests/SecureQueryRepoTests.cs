using Identification.Contracts;
using Microsoft.EntityFrameworkCore;
using MockQueryable;
using MockQueryable.NSubstitute;
using NSubstitute;
using Repository.Contracts;
using Repository.UnitTests.TestDoubles;
using Shouldly;
using Xunit;

namespace Repository.UnitTests.Tests;

public class SecureQueryRepoTests
{
    private const long UserId = 42;

    private readonly DbContext _context = Substitute.For<DbContext>();
    private readonly IIdentityInfo _info = Substitute.For<IIdentityInfo>();
    private readonly IProtected<TestEntity> _lock = Substitute.For<IProtected<TestEntity>>();

    private readonly List<TestEntity> _allData =
    [
        new() { Id = 1, OwnerId = UserId, Name = "Mine" },
        new() { Id = 2, OwnerId = 99, Name = "Someone else" },
    ];

    public SecureQueryRepoTests()
    {
        _info.GetInternalUserId().Returns(UserId);
        _lock.IsMatch(typeof(TestEntity)).Returns(true);
        DbSet<TestEntity> set = _allData.BuildMockDbSet();
        _context.Set<TestEntity>().Returns(set);
    }

    private TestSecureQueryRepo CreateSut(params IProtected[] protection) => new(_context, _info, protection);

    [Fact]
    public async Task GetQueryable_ReturnsSecuredData_WhenLockMatches()
    {
        List<TestEntity> secured = [_allData[0]];
        IQueryable<TestEntity> securedQueryable = secured.BuildMock();
        _lock.Secured(UserId).Returns(securedQueryable);
        TestSecureQueryRepo sut = CreateSut(_lock);

        List<TestEntity> result = await sut.GetQueryable<TestEntity>()
            .ToListAsync(TestContext.Current.CancellationToken);

        result.ShouldBe(secured);
        _lock.Received(1).Secured(UserId);
    }

    [Fact]
    public async Task GetQueryable_ReturnsAllData_WhenNoMatchingProtection()
    {
        TestSecureQueryRepo sut = CreateSut();

        List<TestEntity> result = await sut.GetQueryable<TestEntity>()
            .ToListAsync(TestContext.Current.CancellationToken);

        result.ShouldBe(_allData);
        _lock.DidNotReceive().Secured(Arg.Any<long>());
    }
}

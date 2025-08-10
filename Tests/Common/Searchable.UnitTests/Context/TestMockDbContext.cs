using Microsoft.EntityFrameworkCore;
using Searchable.UnitTests.Models;

namespace Searchable.UnitTests.Context;
public abstract class TestMockDbContext : DbContext
{
    public virtual DbSet<TestArticleForMocking> Articles { get; set; } = null!;
}

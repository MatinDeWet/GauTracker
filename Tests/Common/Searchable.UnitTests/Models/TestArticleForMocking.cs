using NpgsqlTypes;
using Searchable.Domain;

namespace Searchable.UnitTests.Models;
public class TestArticleForMocking : ISearchableEntity
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public NpgsqlTsVector SearchVector { get; set; } = null!;
}

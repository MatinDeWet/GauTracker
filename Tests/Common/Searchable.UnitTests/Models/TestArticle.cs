using System.ComponentModel.DataAnnotations.Schema;
using NpgsqlTypes;
using Searchable.Domain;

namespace Searchable.UnitTests.Models;

public class TestArticle : ISearchableEntity
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    [NotMapped] // This attribute excludes the property from EF Core mapping
    public NpgsqlTsVector SearchVector { get; set; } = null!;
}

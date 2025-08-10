using NpgsqlTypes;

namespace Searchable.Domain;

public interface ISearchableEntity
{
    NpgsqlTsVector SearchVector { get; set; }
}

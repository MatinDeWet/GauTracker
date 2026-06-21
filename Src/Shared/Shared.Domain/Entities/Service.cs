using Ardalis.GuardClauses;
using Domain.Extensions;

namespace Shared.Domain.Entities;

public class Service
{
    public int Id { get; private set; }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public virtual ICollection<StationService> StationServices { get; private set; } = [];

    public static Service Create(int id, string name, string description)
    {
        return new Service
        {
            Id = Guard.Against.NegativeOrZero(id, nameof(id)),
            Name = ValidName(name),
            Description = ValidDescription(description),
        };
    }

    private static string ValidName(string name)
    {
        return Guard.Against.ValidString(name, nameof(name), maxLength: 128);
    }

    private static string ValidDescription(string description)
    {
        return Guard.Against.ValidString(description, nameof(description), maxLength: 512);
    }
}

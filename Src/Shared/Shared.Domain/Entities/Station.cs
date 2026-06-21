using Ardalis.GuardClauses;
using Domain.Extensions;
using Shared.Domain.Enums;

namespace Shared.Domain.Entities;

public class Station
{
    public int Id { get; private set; }

    public string Name { get; private set; }

    public string Address { get; private set; }

    public decimal Latitude { get; private set; }

    public decimal Longitude { get; private set; }

    public StationType StationType { get; private set; }

    public bool IsTerminal { get; private set; }

    public virtual ICollection<StationService> StationServices { get; private set; } = [];

    public static Station Create(int id, string name, string address, decimal latitude, decimal longitude, StationType stationType, bool isTerminal)
    {
        return new Station
        {
            Id = Guard.Against.NegativeOrZero(id, nameof(id)),
            Name = ValidName(name),
            Address = ValidAddress(address),
            Latitude = ValidLatitude(latitude),
            Longitude = ValidLongitude(longitude),
            StationType = stationType,
            IsTerminal = isTerminal,
        };
    }

    private static string ValidName(string name)
    {
        return Guard.Against.ValidString(name, nameof(name), maxLength: 128);
    }

    private static string ValidAddress(string address)
    {
        return Guard.Against.ValidString(address, nameof(address), maxLength: 256);
    }

    private static decimal ValidLatitude(decimal latitude)
    {
        return Guard.Against.OutOfRange(latitude, nameof(latitude), -90m, 90m);
    }

    private static decimal ValidLongitude(decimal longitude)
    {
        return Guard.Against.OutOfRange(longitude, nameof(longitude), -180m, 180m);
    }
}

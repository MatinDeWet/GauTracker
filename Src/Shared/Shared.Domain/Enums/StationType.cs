namespace Shared.Domain.Enums;

/// <summary>
/// Physical configuration of a <see cref="Entities.Station"/> relative to ground level.
/// </summary>
public enum StationType
{
    /// <summary>Built at ground level.</summary>
    AtGrade = 0,

    /// <summary>Raised above ground level (e.g. on a viaduct).</summary>
    Elevated = 1,

    /// <summary>Built below ground level.</summary>
    Underground = 2,
}

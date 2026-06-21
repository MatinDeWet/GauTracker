using Shared.Domain.Entities;
using Shared.Domain.Enums;

namespace Shared.Persistence.Data.Seeding;

internal static class StationSeedData
{
    public static readonly Station[] Stations =
    [
        Station.Create(1, "Hatfield", "Acacia & Grosvenor Street, east of Pretoria CBD", -25.747650m, 28.238000m, StationType.AtGrade, true),
        Station.Create(2, "Pretoria", "Scheiding St, adjacent to Pretoria Main Station", -25.758167m, 28.189467m, StationType.AtGrade, false),
        Station.Create(3, "Centurion", "West Avenue & Gerard Street, near Centurion Lake", -25.851617m, 28.189733m, StationType.Elevated, false),
        Station.Create(4, "Midrand", "Old Pretoria-Johannesburg Road (K101)", -25.996483m, 28.137583m, StationType.AtGrade, false),
        Station.Create(5, "Marlboro", "Cnr Football & Laduma, Alexandra", -26.083917m, 28.111633m, StationType.AtGrade, false),
        Station.Create(6, "Sandton", "Rivonia Road, between West St & Fifth Ave", -26.107800m, 28.057500m, StationType.Underground, false),
        Station.Create(7, "Rosebank", "Oxford Road, between Baker St & Tyrwhitt Ave", -26.145250m, 28.043867m, StationType.Underground, false),
        Station.Create(8, "Park", "Cnr Smit & Wolmarans St, Braamfontein", -26.195533m, 28.041550m, StationType.Underground, true),
        Station.Create(9, "Rhodesfield", "Anson Street, Kempton Park", -26.128333m, 28.225233m, StationType.AtGrade, false),
        Station.Create(10, "OR Tambo", "OR Tambo International Airport, Kempton Park", -26.132933m, 28.231567m, StationType.Elevated, true),
    ];
}

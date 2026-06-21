using Shared.Domain.Entities;

namespace Shared.Persistence.Data.Seeding;

internal static class ServiceSeedData
{
    public static readonly Service[] Services =
    [
        Service.Create(1, "Train service", "Rapid rail access via the personalised Gold Card (tap in / tap out)."),
        Service.Create(2, "Parking", "Secure, access-controlled parking; fee auto-charged to Gold Card on exit."),
        Service.Create(3, "Feeder & distribution buses", "Integrated Gautrain buses connecting the station to surrounding areas."),
    ];
}

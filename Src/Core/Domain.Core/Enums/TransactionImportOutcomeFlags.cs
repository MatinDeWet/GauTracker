namespace Domain.Core.Enums;

[Flags]
public enum TransactionImportOutcomeFlags
{
    None = 0,
    UnknownSites = 1 << 0,
    UnpairedCheckins = 1 << 1,
    OverTimeJourneys = 1 << 2,
    OrphanParkingExits = 1 << 3,
    LowConfidenceBus = 1 << 4,
    PartialSucceeded = 1 << 5
}

using CsvHelper.Configuration;
using Gautrain.Integration.Csv.Models;

namespace Gautrain.Integration.Csv.Mappers;
public sealed class TravelHistoryCsvMap : ClassMap<TravelHistoryCsvModel>
{
    public TravelHistoryCsvMap()
    {
        Map(m => m.SequenceNumber).Name("Sequence Number");
        Map(m => m.TransactionDate).Name("Transaction Date", "Transaction\u00A0Date");
        Map(m => m.Site).Name("Site");
        Map(m => m.TransactionType).Name("Transaction Type");
        Map(m => m.RemainingTrips).Name("Remaining Trips");
        Map(m => m.TransactionValue).Name("Transaction Value");
        Map(m => m.PaygBalance).Name("PAYG Balance");
    }
}

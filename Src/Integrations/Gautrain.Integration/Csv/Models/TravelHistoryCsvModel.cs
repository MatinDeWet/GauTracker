namespace Gautrain.Integration.Csv.Models;
public class TravelHistoryCsvModel
{
    public int SequenceNumber { get; set; }

    public DateTime TransactionDate { get; set; }

    public string Site { get; set; }

    public string TransactionType { get; set; }

    public int RemainingTrips { get; set; }

    public decimal TransactionValue { get; set; }

    public decimal PaygBalance { get; set; }
}

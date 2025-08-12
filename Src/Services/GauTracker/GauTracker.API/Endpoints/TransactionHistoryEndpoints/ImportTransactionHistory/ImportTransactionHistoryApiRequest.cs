namespace GauTracker.API.Endpoints.TransactionHistoryEndpoints.ImportTransactionHistory;

public class ImportTransactionHistoryApiRequest
{
    public IFormFile File { get; set; } = null!;

    public Guid CardId { get; set; }
}

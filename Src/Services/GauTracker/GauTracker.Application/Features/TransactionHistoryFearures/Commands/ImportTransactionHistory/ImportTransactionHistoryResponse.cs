namespace GauTracker.Application.Features.TransactionHistoryFearures.Commands.ImportTransactionHistory;

public sealed record ImportTransactionHistoryResponse
{
    public Guid ImportBatchId { get; init; }
    public string BlobName { get; init; } = default!;
    public string Sha256 { get; init; } = default!;
}

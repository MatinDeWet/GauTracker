using CQRS.Base;

namespace GauTracker.Application.Features.TransactionHistoryFearures.Commands.ImportTransactionHistory;

public record ImportTransactionHistoryRequest : ICommand<ImportTransactionHistoryResponse>
{
    public Stream FileStream { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    public long FileSize { get; set; }

    public Guid CardId { get; set; }
}

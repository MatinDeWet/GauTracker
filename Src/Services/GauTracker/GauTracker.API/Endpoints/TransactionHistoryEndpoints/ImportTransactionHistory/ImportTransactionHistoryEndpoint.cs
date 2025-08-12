using Ardalis.Result;
using CQRS.Core.Contracts;
using FastEndpoints;
using GauTracker.API.Common.Extensions;
using GauTracker.Application.Features.TransactionHistoryFearures.Commands.ImportTransactionHistory;

namespace GauTracker.API.Endpoints.TransactionHistoryEndpoints.ImportTransactionHistory;

public class ImportTransactionHistoryEndpoint(ICommandManager<ImportTransactionHistoryRequest, ImportTransactionHistoryResponse> manager) : Endpoint<ImportTransactionHistoryApiRequest, ImportTransactionHistoryResponse>
{
    public override void Configure()
    {
        Post("/transaction-history/upload");
        AllowFormData();
        AllowFileUploads();
        Summary(s =>
        {
            s.Summary = "Upload a travel transaction file";
            s.Description = "Uploads a CSV file containing travel transaction data to Azure Blob Storage. The file will be stored securely with metadata for future processing.";
        });
    }

    public override async Task HandleAsync(ImportTransactionHistoryApiRequest req, CancellationToken ct)
    {
        // Map API request to Application request
        using Stream fileStream = req.File.OpenReadStream();

        var applicationRequest = new ImportTransactionHistoryRequest
        {
            FileStream = fileStream,
            FileName = req.File.FileName ?? $"travel-transaction-{DateTime.UtcNow:yyyyMMddHHmmss}.csv",
            ContentType = req.File.ContentType ?? "text/csv",
            FileSize = req.File.Length,
            CardId = req.CardId
        };

        Result<ImportTransactionHistoryResponse> result = await manager.Handle(applicationRequest, ct);

        await this.SendResponse(result, response => response.GetValue());
    }
}

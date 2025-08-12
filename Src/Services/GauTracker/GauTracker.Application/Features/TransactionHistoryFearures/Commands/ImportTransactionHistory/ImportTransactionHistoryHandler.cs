using System.Globalization;
using System.Security.Cryptography;
using Ardalis.Result;
using BlobStorage.Common.Constants;
using BlobStorage.Contracts;
using CQRS.Core.Contracts;
using Domain.Core.Entities;
using GauTracker.Application.Repositories.Command;
using GauTracker.Application.Repositories.Query;
using Microsoft.EntityFrameworkCore;

namespace GauTracker.Application.Features.TransactionHistoryFearures.Commands.ImportTransactionHistory;

internal sealed class ImportTransactionHistoryHandler(
    IBlobService blobService,
    ICardQueryRepository cardQueryRepo,
    ITransactionHistoryImportBatchQueryRepository importBatchQueryRepo,
    ITransactionHistoryImportBatchCommandRepository importCommandRepo
    ) : ICommandManager<ImportTransactionHistoryRequest, ImportTransactionHistoryResponse>
{
    private const int MaxMemoryStreamSizeBytes = 16 * 1024 * 1024; // 16MB

    public async Task<Result<ImportTransactionHistoryResponse>> Handle(ImportTransactionHistoryRequest command, CancellationToken cancellationToken)
    {
        // Verify card exists
        bool hasCard = await cardQueryRepo.Cards
            .Where(x => x.Id == command.CardId)
            .AnyAsync(cancellationToken);

        if (!hasCard)
        {
            return Result.NotFound($"Card with ID {command.CardId} does not exist.");
        }

        // Prepare stream for hash calculation
        Stream uploadStream = await PrepareUploadStream(command, cancellationToken);

        // Calculate file hash
        string sha256Hex = await CalculateFileHash(uploadStream, cancellationToken);

        // Check for duplicate uploads
        bool duplicateUpload = await importBatchQueryRepo.TransactionHistoryImportBatchs
            .Where(x => x.Sha256 == sha256Hex)
            .AnyAsync(cancellationToken);

        if (duplicateUpload)
        {
            return Result.Conflict("This file has already been uploaded.");
        }

        // Upload to blob storage
        uploadStream.Position = 0;
        string blobName = await UploadToBlobStorage(command, uploadStream, cancellationToken);

        // Create and save batch record
        var batch = TransactionHistoryImportBatch.Create(
            command.CardId,
            ContainerNameConstants.TransportImport,
            blobName,
            sha256Hex);

        await importCommandRepo.InsertAsync(batch, true, cancellationToken);

        return Result.Success(new ImportTransactionHistoryResponse
        {
            ImportBatchId = batch.Id,
            BlobName = blobName,
            Sha256 = sha256Hex
        });
    }

    private async Task<Stream> PrepareUploadStream(ImportTransactionHistoryRequest command, CancellationToken cancellationToken)
    {
        Stream uploadStream = command.FileStream;

        if (!command.FileStream.CanSeek)
        {
            int memoryStreamSize = (int)Math.Min(command.FileSize, MaxMemoryStreamSizeBytes);
            var ms = new MemoryStream(memoryStreamSize);
            await command.FileStream.CopyToAsync(ms, cancellationToken);
            ms.Position = 0;
            uploadStream = ms;
        }

        return uploadStream;
    }

    private async Task<string> CalculateFileHash(Stream stream, CancellationToken cancellationToken)
    {
        stream.Position = 0;
        using var sha = SHA256.Create();
        byte[] hash = await sha.ComputeHashAsync(stream, cancellationToken);
        return Convert.ToHexString(hash);
    }

    private async Task<string> UploadToBlobStorage(ImportTransactionHistoryRequest command, Stream uploadStream, CancellationToken cancellationToken)
    {
        var metadata = new Dictionary<string, string>
        {
            ["fileName"] = command.FileName,
            ["contentType"] = command.ContentType,
            ["fileSize"] = command.FileSize.ToString(CultureInfo.InvariantCulture),
            ["cardId"] = command.CardId.ToString(),
            ["uploadedAt"] = DateTimeOffset.UtcNow.ToString("O")
        };

        return await blobService.UploadFileAsync(
            uploadStream,
            command.FileName,
            ContainerNameConstants.TransportImport,
            metadata,
            cancellationToken);
    }
}

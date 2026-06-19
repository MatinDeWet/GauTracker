using System.Globalization;
using Ardalis.Result;
using BlobStorage.Contracts;
using BlobStorage.Contracts.Models;
using CQRS.Core.Contracts;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.Common.Constants;
using WebApi.Application.Repositories.CommandRepos.SecuredRepos;
using WebApi.Application.Repositories.QueryRepos.SecuredRepos;
using Shared.Domain.Entities;

namespace WebApi.Application.Features.TravelHistoryFileFeatures.CreateTravelHistoryFile;

internal sealed class CreateTravelHistoryFileCommandHandler(
    ISecuredQueryRepo queryRepo,
    ISecuredCommandRepo commandRepo,
    IBlobStorageService blobStorage) : ICommandManager<CreateTravelHistoryFileCommand, CreateTravelHistoryFileResponse>
{
    public async Task<Result<CreateTravelHistoryFileResponse>> Handle(CreateTravelHistoryFileCommand request, CancellationToken cancellationToken)
    {
        bool cardExists = await queryRepo.Cards
            .AnyAsync(c => c.Id == request.CardId, cancellationToken);

        if (!cardExists)
        {
            return Result.NotFound();
        }

        if (!CsvFile.HasCsvExtension(request.FileName))
        {
            return Result.Invalid(new ValidationError("File name must be a .csv file."));
        }

        string contentType = CsvFile.NormalizeContentType(request.ContentType);

        if (!CsvFile.IsAllowedContentType(contentType))
        {
            return Result.Invalid(new ValidationError("Content type must be a CSV content type."));
        }

        string container = BlobContainers.ForYear(DateTimeOffset.UtcNow.Year);
        string blobKey = string.Create(CultureInfo.InvariantCulture, $"{request.CardId}/{Guid.NewGuid():N}.csv");

        var file = TravelHistoryFile.Create(
            request.CardId,
            request.FileName,
            contentType,
            container,
            blobKey,
            request.DisplayName);

        await commandRepo.InsertAsync(file, persistImmediately: true, cancellationToken);

        BlobUploadTicket ticket = await blobStorage.CreateUploadUrlAsync(container, blobKey, cancellationToken: cancellationToken);

        return new CreateTravelHistoryFileResponse(file.Id, container, blobKey, ticket.UploadUrl, ticket.ExpiresAt);
    }
}

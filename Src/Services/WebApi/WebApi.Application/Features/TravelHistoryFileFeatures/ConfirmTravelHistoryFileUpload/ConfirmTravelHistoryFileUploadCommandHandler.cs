using Ardalis.Result;
using BlobStorage.Contracts;
using BlobStorage.Contracts.Models;
using CQRS.Core.Contracts;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.Repositories.CommandRepos.SecuredRepos;
using WebApi.Application.Repositories.QueryRepos.SecuredRepos;
using WebApi.Domain.Entities;

namespace WebApi.Application.Features.TravelHistoryFileFeatures.ConfirmTravelHistoryFileUpload;

internal sealed class ConfirmTravelHistoryFileUploadCommandHandler(
    ISecuredQueryRepo queryRepo,
    ISecuredCommandRepo commandRepo,
    IBlobStorageService blobStorage) : ICommandManager<ConfirmTravelHistoryFileUploadCommand>
{
    public async Task<Result> Handle(ConfirmTravelHistoryFileUploadCommand request, CancellationToken cancellationToken)
    {
        TravelHistoryFile? file = await queryRepo.TravelHistoryFiles
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (file is null)
        {
            return Result.NotFound();
        }

        BlobMetadata? metadata = await blobStorage.GetMetadataAsync(file.BlobContainer, file.BlobKey, cancellationToken);

        if (metadata is null)
        {
            return Result.Invalid(new ValidationError("The file has not been uploaded to storage yet."));
        }

        file.MarkUploaded(metadata.ContentLength);

        await commandRepo.UpdateAsync(file, persistImmediately: true, cancellationToken);

        return Result.Success();
    }
}

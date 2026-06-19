using Ardalis.Result;
using BlobStorage.Contracts;
using CQRS.Core.Contracts;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.Repositories.CommandRepos.SecuredRepos;
using WebApi.Application.Repositories.QueryRepos.SecuredRepos;
using Shared.Domain.Entities;

namespace WebApi.Application.Features.TravelHistoryFileFeatures.DeleteTravelHistoryFile;

internal sealed class DeleteTravelHistoryFileCommandHandler(
    ISecuredQueryRepo queryRepo,
    ISecuredCommandRepo commandRepo,
    IBlobStorageService blobStorage) : ICommandManager<DeleteTravelHistoryFileCommand>
{
    public async Task<Result> Handle(DeleteTravelHistoryFileCommand request, CancellationToken cancellationToken)
    {
        TravelHistoryFile? file = await queryRepo.TravelHistoryFiles
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (file is null)
        {
            return Result.NotFound();
        }

        await blobStorage.DeleteAsync(file.BlobContainer, file.BlobKey, cancellationToken);

        await commandRepo.DeleteAsync(file, persistImmediately: true, cancellationToken);

        return Result.Success();
    }
}

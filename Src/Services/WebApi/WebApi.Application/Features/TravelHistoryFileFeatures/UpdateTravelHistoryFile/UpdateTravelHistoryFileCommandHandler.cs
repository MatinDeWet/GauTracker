using Ardalis.Result;
using CQRS.Core.Contracts;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.Repositories.CommandRepos.SecuredRepos;
using WebApi.Application.Repositories.QueryRepos.SecuredRepos;
using WebApi.Domain.Entities;

namespace WebApi.Application.Features.TravelHistoryFileFeatures.UpdateTravelHistoryFile;

internal sealed class UpdateTravelHistoryFileCommandHandler(
    ISecuredQueryRepo queryRepo,
    ISecuredCommandRepo commandRepo) : ICommandManager<UpdateTravelHistoryFileCommand>
{
    public async Task<Result> Handle(UpdateTravelHistoryFileCommand request, CancellationToken cancellationToken)
    {
        if (!CsvFile.HasCsvExtension(request.FileName))
        {
            return Result.Invalid(new ValidationError("File name must be a .csv file."));
        }

        TravelHistoryFile? file = await queryRepo.TravelHistoryFiles
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (file is null)
        {
            return Result.NotFound();
        }

        file.Update(request.FileName, request.DisplayName);

        await commandRepo.UpdateAsync(file, persistImmediately: true, cancellationToken);

        return Result.Success();
    }
}

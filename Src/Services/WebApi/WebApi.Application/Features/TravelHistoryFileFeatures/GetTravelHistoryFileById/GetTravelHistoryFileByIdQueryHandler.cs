using Ardalis.Result;
using CQRS.Core.Contracts;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.Repositories.QueryRepos.SecuredRepos;

namespace WebApi.Application.Features.TravelHistoryFileFeatures.GetTravelHistoryFileById;

internal sealed class GetTravelHistoryFileByIdQueryHandler(ISecuredQueryRepo queryRepo)
    : IQueryManager<GetTravelHistoryFileByIdQuery, GetTravelHistoryFileByIdResponse>
{
    public async Task<Result<GetTravelHistoryFileByIdResponse>> Handle(GetTravelHistoryFileByIdQuery request, CancellationToken cancellationToken)
    {
        GetTravelHistoryFileByIdResponse? file = await queryRepo.TravelHistoryFiles
            .Where(x => x.Id == request.Id)
            .Select(x => new GetTravelHistoryFileByIdResponse(
                x.Id,
                x.CardId,
                x.FileName,
                x.DisplayName,
                x.ContentType,
                x.SizeInBytes,
                x.Status,
                x.DateUploaded,
                x.DateCreated))
            .FirstOrDefaultAsync(cancellationToken);

        if (file is null)
        {
            return Result.NotFound();
        }

        return file;
    }
}

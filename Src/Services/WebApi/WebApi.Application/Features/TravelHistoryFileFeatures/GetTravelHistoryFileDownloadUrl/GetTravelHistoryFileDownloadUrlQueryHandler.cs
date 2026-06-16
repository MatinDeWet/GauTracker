using Ardalis.Result;
using BlobStorage.Contracts;
using BlobStorage.Contracts.Models;
using CQRS.Core.Contracts;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.Repositories.QueryRepos.SecuredRepos;

namespace WebApi.Application.Features.TravelHistoryFileFeatures.GetTravelHistoryFileDownloadUrl;

internal sealed class GetTravelHistoryFileDownloadUrlQueryHandler(
    ISecuredQueryRepo queryRepo,
    IBlobStorageService blobStorage) : IQueryManager<GetTravelHistoryFileDownloadUrlQuery, GetTravelHistoryFileDownloadUrlResponse>
{
    public async Task<Result<GetTravelHistoryFileDownloadUrlResponse>> Handle(GetTravelHistoryFileDownloadUrlQuery request, CancellationToken cancellationToken)
    {
        var file = await queryRepo.TravelHistoryFiles
            .Where(x => x.Id == request.Id)
            .Select(x => new { x.FileName, x.BlobContainer, x.BlobKey })
            .FirstOrDefaultAsync(cancellationToken);

        if (file is null)
        {
            return Result.NotFound();
        }

        BlobDownloadTicket ticket = blobStorage.CreateDownloadUrl(file.BlobContainer, file.BlobKey);

        return new GetTravelHistoryFileDownloadUrlResponse(file.FileName, ticket.DownloadUrl, ticket.ExpiresAt);
    }
}

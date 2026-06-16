using Ardalis.Result;
using BlobStorage.Contracts;
using BlobStorage.Contracts.Models;
using CQRS.Core.Contracts;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.Repositories.QueryRepos.SecuredRepos;

namespace WebApi.Application.Features.TravelHistoryFileFeatures.GetTravelHistoryFileUploadUrl;

internal sealed class GetTravelHistoryFileUploadUrlQueryHandler(
    ISecuredQueryRepo queryRepo,
    IBlobStorageService blobStorage) : IQueryManager<GetTravelHistoryFileUploadUrlQuery, GetTravelHistoryFileUploadUrlResponse>
{
    public async Task<Result<GetTravelHistoryFileUploadUrlResponse>> Handle(GetTravelHistoryFileUploadUrlQuery request, CancellationToken cancellationToken)
    {
        var file = await queryRepo.TravelHistoryFiles
            .Where(x => x.Id == request.Id)
            .Select(x => new { x.BlobContainer, x.BlobKey })
            .FirstOrDefaultAsync(cancellationToken);

        if (file is null)
        {
            return Result.NotFound();
        }

        BlobUploadTicket ticket = await blobStorage.CreateUploadUrlAsync(file.BlobContainer, file.BlobKey, cancellationToken: cancellationToken);

        return new GetTravelHistoryFileUploadUrlResponse(ticket.Key, ticket.UploadUrl, ticket.ExpiresAt);
    }
}

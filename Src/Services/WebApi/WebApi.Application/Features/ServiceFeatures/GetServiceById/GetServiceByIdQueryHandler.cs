using Ardalis.Result;
using CQRS.Core.Contracts;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.Repositories.QueryRepos.UnsecuredRepos;

namespace WebApi.Application.Features.ServiceFeatures.GetServiceById;

internal sealed class GetServiceByIdQueryHandler(IUnsecuredQueryRepo queryRepo)
    : IQueryManager<GetServiceByIdQuery, GetServiceByIdResponse>
{
    public async Task<Result<GetServiceByIdResponse>> Handle(GetServiceByIdQuery request, CancellationToken cancellationToken)
    {
        GetServiceByIdResponse? service = await queryRepo.Services
            .Where(x => x.Id == request.Id)
            .Select(x => new GetServiceByIdResponse(x.Id, x.Name, x.Description))
            .FirstOrDefaultAsync(cancellationToken);

        if (service is null)
        {
            return Result.NotFound();
        }

        return service;
    }
}

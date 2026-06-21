using Ardalis.Result;
using CQRS.Core.Contracts;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.Repositories.QueryRepos.UnsecuredRepos;

namespace WebApi.Application.Features.ServiceFeatures.ListServices;

internal sealed class ListServicesQueryHandler(IUnsecuredQueryRepo queryRepo)
    : IQueryManager<ListServicesQuery, IReadOnlyList<ListServicesResponse>>
{
    public async Task<Result<IReadOnlyList<ListServicesResponse>>> Handle(ListServicesQuery request, CancellationToken cancellationToken)
    {
        List<ListServicesResponse> services = await queryRepo.Services
            .Select(x => new ListServicesResponse
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
            })
            .ToListAsync(cancellationToken);

        return services;
    }
}

using Repository.Implementation;
using WebApi.Application.Repositories.QueryRepos.UnsecuredRepos;
using Shared.Domain.Entities;
using Shared.Persistence.Data.Contexts;

namespace WebApi.infrastructure.Repositories.QueryRepos.UnsecuredRepos;

internal sealed class UnsecuredQueryRepo(GauContext context)
    : QueryRepo<GauContext>(context), IUnsecuredQueryRepo
{
    public IQueryable<User> Users => GetQueryable<User>();

    public IQueryable<Station> Stations => GetQueryable<Station>();

    public IQueryable<Service> Services => GetQueryable<Service>();
}

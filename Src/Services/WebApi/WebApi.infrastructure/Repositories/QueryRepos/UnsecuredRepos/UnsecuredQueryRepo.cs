using Repository.Implementation;
using WebApi.Application.Repositories.QueryRepos.UnsecuredRepos;
using WebApi.Domain.Entities;
using WebApi.infrastructure.Data.Contexts;

namespace WebApi.infrastructure.Repositories.QueryRepos.UnsecuredRepos;

internal sealed class UnsecuredQueryRepo(GauContext context)
    : QueryRepo<GauContext>(context), IUnsecuredQueryRepo
{
    public IQueryable<User> Users => GetQueryable<User>();
}

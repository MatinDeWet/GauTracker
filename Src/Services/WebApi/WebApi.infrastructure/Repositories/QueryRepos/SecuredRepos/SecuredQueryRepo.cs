using Identification.Contracts;
using Repository.Contracts;
using Repository.Implementation;
using WebApi.Application.Repositories.QueryRepos.SecuredRepos;
using WebApi.Domain.Entities;
using WebApi.infrastructure.Data.Contexts;

namespace WebApi.infrastructure.Repositories.QueryRepos.SecuredRepos;

internal sealed class SecuredQueryRepo(GauContext context, IIdentityInfo info, IEnumerable<IProtected> protection)
    : SecureQueryRepo<GauContext>(context, info, protection), ISecuredQueryRepo
{
    public IQueryable<User> Users => GetQueryable<User>();
}

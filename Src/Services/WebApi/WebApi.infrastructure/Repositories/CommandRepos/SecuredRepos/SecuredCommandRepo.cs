using Identification.Contracts;
using Repository.Contracts;
using Repository.Implementation;
using WebApi.Application.Repositories.CommandRepos.SecuredRepos;
using WebApi.infrastructure.Data.Contexts;

namespace WebApi.infrastructure.Repositories.CommandRepos.SecuredRepos;

internal sealed class SecuredCommandRepo(GauContext context, IIdentityInfo info, IEnumerable<IProtected> protection)
    : SecureCommandRepo<GauContext>(context, info, protection), ISecuredCommandRepo;

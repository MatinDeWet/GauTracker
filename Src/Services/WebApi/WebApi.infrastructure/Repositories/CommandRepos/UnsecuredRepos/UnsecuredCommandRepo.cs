using Repository.Implementation;
using WebApi.Application.Repositories.CommandRepos.UnsecuredRepos;
using WebApi.infrastructure.Data.Contexts;

namespace WebApi.infrastructure.Repositories.CommandRepos.UnsecuredRepos;

internal sealed class UnsecuredCommandRepo(GauContext context)
    : CommandRepo<GauContext>(context), IUnsecuredCommandRepo;

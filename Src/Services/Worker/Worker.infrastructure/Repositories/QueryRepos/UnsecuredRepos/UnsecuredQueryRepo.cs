using Repository.Implementation;
using Shared.Domain.Entities;
using Shared.Persistence.Data.Contexts;
using Worker.Application.Repositories.QueryRepos.UnsecuredRepos;

namespace Worker.infrastructure.Repositories.QueryRepos.UnsecuredRepos;

internal sealed class UnsecuredQueryRepo(GauContext context)
    : QueryRepo<GauContext>(context), IUnsecuredQueryRepo
{
    public IQueryable<User> Users => GetQueryable<User>();

    public IQueryable<Card> Cards => GetQueryable<Card>();

    public IQueryable<TravelHistoryFile> TravelHistoryFiles => GetQueryable<TravelHistoryFile>();
}

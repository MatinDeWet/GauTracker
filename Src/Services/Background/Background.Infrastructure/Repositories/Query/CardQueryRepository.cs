using Background.Application.Repositories.Query;
using Background.Infrastructure.Data.Contexts;
using Domain.Core.Entities;
using Repository.Core.Implementation;

namespace Background.Infrastructure.Repositories.Query;
internal sealed class CardQueryRepository : QueryRepo<GauTrackerContext>, ICardQueryRepository
{
    public CardQueryRepository(GauTrackerContext context)
        : base(context)
    {
    }

    public IQueryable<Card> Cards => GetQueryable<Card>();
}

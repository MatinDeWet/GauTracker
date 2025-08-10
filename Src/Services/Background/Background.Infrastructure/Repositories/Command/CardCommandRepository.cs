using Background.Application.Repositories.Command;
using Background.Infrastructure.Data.Contexts;
using Repository.Core.Implementation;

namespace Background.Infrastructure.Repositories.Command;
internal sealed class CardCommandRepository : CommandRepo<GauTrackerContext>, ICardCommandRepository
{
    public CardCommandRepository(GauTrackerContext context)
        : base(context)
    {
    }
}

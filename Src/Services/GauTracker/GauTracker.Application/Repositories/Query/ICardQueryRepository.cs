using Domain.Core.Entities;
using Repository.Base;

namespace GauTracker.Application.Repositories.Query;
public interface ICardQueryRepository : ISecureQueryRepo
{
    IQueryable<Card> Cards { get; }

    Task<bool> NumberExists(string number, CancellationToken cancellationToken);
}

using Domain.Core.Entities;
using Repository.Base;

namespace Background.Application.Repositories.Query;
public interface ICardQueryRepository : IQueryRepo
{
    IQueryable<Card> Cards { get; }
}

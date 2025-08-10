using Ardalis.Result;
using CQRS.Base;

namespace CQRS.Core.Contracts;
public interface IQueryManager<in TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken);
}

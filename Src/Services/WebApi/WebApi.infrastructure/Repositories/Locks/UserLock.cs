using Repository.Enums;
using Repository.Lock;
using WebApi.Domain.Entities;
using WebApi.infrastructure.Data.Contexts;

namespace WebApi.infrastructure.Repositories.Locks;

/// <summary>
/// Row-level protection for <see cref="User"/>: a user may only see and modify their own record.
/// </summary>
internal sealed class UserLock(GauContext context) : Lock<User>
{
    public override IQueryable<User> Secured(long userId)
    {
        return context.Set<User>().Where(x => x.Id == userId);
    }

    public override Task<bool> HasAccess(User obj, long userId, RepositoryOperationEnum operation, CancellationToken cancellationToken)
    {
        return Task.FromResult(obj.Id == userId);
    }
}

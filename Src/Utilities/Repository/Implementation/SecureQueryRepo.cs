using Identification.Contracts;
using Microsoft.EntityFrameworkCore;
using Repository.Contracts;

namespace Repository.Implementation;

/// <summary>
/// Provides secure query operations by applying entity-specific security filters based on user identity.
/// This class acts as a security gateway for read operations, automatically filtering queryable collections
/// to ensure users can only access data they have permission to view through registered entity protection
/// implementations and optional access rules.
/// </summary>
/// <typeparam name="TCtx">The type of Entity Framework DbContext.</typeparam>
public abstract class SecureQueryRepo<TCtx> : QueryRepo<TCtx>, ISecureQueryRepo where TCtx : DbContext
{
    protected readonly IIdentityInfo _info;

    private readonly IEnumerable<IProtected> _protection;

    /// <summary>
    /// Initializes a new instance of the <see cref="SecureQueryRepo{TCtx}"/> class.
    /// </summary>
    /// <param name="context">The Entity Framework DbContext.</param>
    /// <param name="info">The identity information provider.</param>
    /// <param name="protection">Collection of entity protection implementations.</param>
    protected SecureQueryRepo(TCtx context, IIdentityInfo info, IEnumerable<IProtected> protection) : base(context)
    {
        _info = info;
        _protection = protection;
    }

    /// <summary>
    /// Returns a secure queryable collection with appropriate security filters applied
    /// based on the configured access requirements and entity protections.
    /// This is the secure version of <see cref="QueryRepo{TCtx}.GetQueryable{T}"/> with automatic security filtering.
    /// First evaluates access rules if configured, then applies entity-specific protection filters.
    /// </summary>
    /// <typeparam name="T">The type of entity to query.</typeparam>
    /// <returns>A filtered <see cref="IQueryable{T}"/> or unfiltered if no protection is configured.</returns>
    public new IQueryable<T> GetQueryable<T>() where T : class
    {
        if (_protection.FirstOrDefault(x => x.IsMatch(typeof(T))) is IProtected<T> entityLock)
        {
            return entityLock.Secured(_info.GetInternalUserId())
                .AsNoTracking()
                .AsSingleQuery();
        }

        return base.GetQueryable<T>();
    }
}

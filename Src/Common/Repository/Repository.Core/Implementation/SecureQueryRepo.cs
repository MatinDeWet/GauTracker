using Identification.Base;
using Microsoft.EntityFrameworkCore;
using Repository.Base;
using Repository.Core.Contracts;

namespace Repository.Core.Implementation;

/// <summary>
/// Provides secure query operations by applying entity-specific security filters based on user identity.
/// This class acts as a security gateway for read operations, automatically filtering queryable collections
/// to ensure users can only access data they have permission to view through registered entity protection implementations.
/// </summary>
/// <typeparam name="TCtx">The type of Entity Framework DbContext.</typeparam>
public class SecureQueryRepo<TCtx> : ISecureQueryRepo where TCtx : DbContext
{
    protected readonly TCtx _context;
    protected readonly IIdentityInfo _info;

    private readonly IEnumerable<IProtected> _protection;

    /// <summary>
    /// Initializes a new instance of the <see cref="SecureQueryRepo{TCtx}"/> class.
    /// </summary>
    /// <param name="context">The Entity Framework DbContext.</param>
    /// <param name="info">The identity information provider.</param>
    /// <param name="protection">Collection of entity protection implementations.</param>
    public SecureQueryRepo(TCtx context, IIdentityInfo info, IEnumerable<IProtected> protection)
    {
        _context = context;
        _info = info;
        _protection = protection;
    }

    /// <summary>
    /// Returns a secure queryable collection with appropriate security filters applied
    /// based on the configured access requirements.
    /// </summary>
    /// <typeparam name="T">The type of entity to query.</typeparam>
    /// <returns>A filtered <see cref="IQueryable{T}"/> or unfiltered if no protection is configured.</returns>
    public IQueryable<T> Secure<T>() where T : class
    {
        if (_protection.FirstOrDefault(x => x.IsMatch(typeof(T))) is IProtected<T> entityLock)
        {
            return entityLock.Secured(_info.GetIdentityId());
        }

        return _context.Set<T>();
    }

    /// <summary>
    /// Returns a queryable collection for the specified entity type without any security filtering.
    /// </summary>
    /// <typeparam name="T">The type of entity to query.</typeparam>
    /// <returns>An <see cref="IQueryable{T}"/> for the specified entity type.</returns>
    public IQueryable<T> GetQueryable<T>() where T : class
    {
        return _context.Set<T>();
    }
}

using Identification.Base;
using Microsoft.EntityFrameworkCore;
using Repository.Base;
using Repository.Core.Contracts;
using Repository.Core.Enums;

namespace Repository.Core.Implementation;

/// <summary>
/// Provides secure command operations by validating user permissions before executing data modifications.
/// This class acts as a security gateway for write operations (insert, update, delete), ensuring all data
/// modification attempts are authorized through entity-specific protection implementations before being
/// staged to the database context.
/// </summary>
/// <typeparam name="TCtx">The type of Entity Framework DbContext.</typeparam>
public class SecureCommandRepo<TCtx> : ISecureCommandRepo where TCtx : DbContext
{
    protected readonly TCtx _context;
    protected readonly IIdentityInfo _info;

    private readonly IEnumerable<IProtected> _protection;

    /// <summary>
    /// Initializes a new instance of the <see cref="SecureCommandRepo{TCtx}"/> class.
    /// </summary>
    /// <param name="context">The Entity Framework DbContext.</param>
    /// <param name="info">The identity information provider.</param>
    /// <param name="protection">Collection of entity protection implementations.</param>
    public SecureCommandRepo(TCtx context, IIdentityInfo info, IEnumerable<IProtected> protection)
    {
        _context = context;
        _info = info;
        _protection = protection;
    }

    /// <summary>
    /// Validates permissions and stages the entity for insertion.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="obj">The entity to insert.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="UnauthorizedAccessException">Thrown when access is denied.</exception>
    public virtual async Task InsertAsync<T>(T obj, CancellationToken cancellationToken) where T : class
    {
        bool hasAccess = await HasAccess(obj, RepositoryOperationEnum.Insert, cancellationToken);

        if (!hasAccess)
        {
            throw new UnauthorizedAccessException();
        }

        _context.Add(obj);
    }

    /// <summary>
    /// Validates permissions and stages the entity for insertion, optionally persisting immediately.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="obj">The entity to insert.</param>
    /// <param name="persistImmediately">Whether to save changes immediately.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="UnauthorizedAccessException">Thrown when access is denied.</exception>
    public async Task InsertAsync<T>(T obj, bool persistImmediately, CancellationToken cancellationToken) where T : class
    {
        await InsertAsync(obj, cancellationToken);

        if (persistImmediately)
        {
            await SaveAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Validates permissions and stages the entity for update.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="obj">The entity to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="UnauthorizedAccessException">Thrown when access is denied.</exception>
    public virtual async Task UpdateAsync<T>(T obj, CancellationToken cancellationToken) where T : class
    {
        bool hasAccess = await HasAccess(obj, RepositoryOperationEnum.Update, cancellationToken);

        if (!hasAccess)
        {
            throw new UnauthorizedAccessException();
        }

        _context.Update(obj);
    }

    /// <summary>
    /// Validates permissions and stages the entity for update, optionally persisting immediately.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="obj">The entity to update.</param>
    /// <param name="persistImmediately">Whether to save changes immediately.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="UnauthorizedAccessException">Thrown when access is denied.</exception>
    public async Task UpdateAsync<T>(T obj, bool persistImmediately, CancellationToken cancellationToken) where T : class
    {
        await UpdateAsync(obj, cancellationToken);

        if (persistImmediately)
        {
            await SaveAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Validates permissions and stages the entity for deletion.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="obj">The entity to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="UnauthorizedAccessException">Thrown when access is denied.</exception>
    public virtual async Task DeleteAsync<T>(T obj, CancellationToken cancellationToken) where T : class
    {
        bool hasAccess = await HasAccess(obj, RepositoryOperationEnum.Delete, cancellationToken);

        if (!hasAccess)
        {
            throw new UnauthorizedAccessException();
        }

        _context.Remove(obj);
    }

    /// <summary>
    /// Validates permissions and stages the entity for deletion, optionally persisting immediately.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="obj">The entity to delete.</param>
    /// <param name="persistImmediately">Whether to save changes immediately.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="UnauthorizedAccessException">Thrown when access is denied.</exception>
    public async Task DeleteAsync<T>(T obj, bool persistImmediately, CancellationToken cancellationToken) where T : class
    {
        await DeleteAsync(obj, cancellationToken);

        if (persistImmediately)
        {
            await SaveAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Persists all staged changes to the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    public virtual async Task SaveAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Validates user access for the specified operation on the entity.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="obj">The entity to check.</param>
    /// <param name="operation">The operation type.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if access is granted; otherwise, <c>false</c>.</returns>
    private async Task<bool> HasAccess<T>(T obj, RepositoryOperationEnum operation, CancellationToken cancellationToken) where T : class
    {
        bool result = true;

        if (_protection.FirstOrDefault(x => x.IsMatch(typeof(T))) is IProtected<T> entityLock)
        {
            result = await entityLock.HasAccess(obj, _info.GetIdentityId(), operation, cancellationToken);
        }

        return result;
    }
}


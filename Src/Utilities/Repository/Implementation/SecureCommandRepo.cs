using Identification.Contracts;
using Microsoft.EntityFrameworkCore;
using Repository.Contracts;
using Repository.Enums;

namespace Repository.Implementation;

/// <summary>
/// Provides secure command operations by validating user permissions before executing data modifications.
/// This class acts as a security gateway for write operations (insert, update, delete), ensuring all data
/// modification attempts are authorized through entity-specific protection implementations and optional
/// access rules before being staged to the database context.
/// </summary>
/// <typeparam name="TCtx">The type of Entity Framework DbContext.</typeparam>
public abstract class SecureCommandRepo<TCtx> : CommandRepo<TCtx>, ISecureCommandRepo where TCtx : DbContext
{
    protected readonly IIdentityInfo _info;

    private readonly IEnumerable<IProtected> _protection;

    /// <summary>
    /// Initializes a new instance of the <see cref="SecureCommandRepo{TCtx}"/> class.
    /// </summary>
    /// <param name="context">The Entity Framework DbContext.</param>
    /// <param name="info">The identity information provider.</param>
    /// <param name="protection">Collection of entity protection implementations.</param>
    protected SecureCommandRepo(TCtx context, IIdentityInfo info, IEnumerable<IProtected> protection) : base(context)
    {
        _info = info;
        _protection = protection;
    }

    /// <summary>
    /// Validates permissions and stages the entity for insertion.
    /// This is the secure version of <see cref="CommandRepo{TCtx}.InsertAsync{T}(T, CancellationToken)"/> with built-in permission validation.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="obj">The entity to insert.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user does not have permission to insert the entity.</exception>
    public new virtual async Task InsertAsync<T>(T obj, CancellationToken cancellationToken) where T : class
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
    /// This is the secure version of <see cref="CommandRepo{TCtx}.InsertAsync{T}(T, bool, CancellationToken)"/> with built-in permission validation.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="obj">The entity to insert.</param>
    /// <param name="persistImmediately">Whether to save changes immediately.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user does not have permission to insert the entity.</exception>
    public new async Task InsertAsync<T>(T obj, bool persistImmediately, CancellationToken cancellationToken) where T : class
    {
        await InsertAsync(obj, cancellationToken);

        if (persistImmediately)
        {
            await SaveAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Validates permissions and stages the entity for update.
    /// This is the secure version of <see cref="CommandRepo{TCtx}.UpdateAsync{T}(T, CancellationToken)"/> with built-in permission validation.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="obj">The entity to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user does not have permission to update the entity.</exception>
    public new virtual async Task UpdateAsync<T>(T obj, CancellationToken cancellationToken) where T : class
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
    /// This is the secure version of <see cref="CommandRepo{TCtx}.UpdateAsync{T}(T, bool, CancellationToken)"/> with built-in permission validation.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="obj">The entity to update.</param>
    /// <param name="persistImmediately">Whether to save changes immediately.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user does not have permission to update the entity.</exception>
    public new async Task UpdateAsync<T>(T obj, bool persistImmediately, CancellationToken cancellationToken) where T : class
    {
        await UpdateAsync(obj, cancellationToken);

        if (persistImmediately)
        {
            await SaveAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Validates permissions and stages the entity for deletion.
    /// This is the secure version of <see cref="CommandRepo{TCtx}.DeleteAsync{T}(T, CancellationToken)"/> with built-in permission validation.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="obj">The entity to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user does not have permission to delete the entity.</exception>
    public new virtual async Task DeleteAsync<T>(T obj, CancellationToken cancellationToken) where T : class
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
    /// This is the secure version of <see cref="CommandRepo{TCtx}.DeleteAsync{T}(T, bool, CancellationToken)"/> with built-in permission validation.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="obj">The entity to delete.</param>
    /// <param name="persistImmediately">Whether to save changes immediately.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user does not have permission to delete the entity.</exception>
    public new async Task DeleteAsync<T>(T obj, bool persistImmediately, CancellationToken cancellationToken) where T : class
    {
        await DeleteAsync(obj, cancellationToken);

        if (persistImmediately)
        {
            await SaveAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Validates user access for the specified operation on the entity.
    /// First evaluates any configured access rules, then checks entity-specific protection if available.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="obj">The entity to check.</param>
    /// <param name="operation">The operation type.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if access is granted; otherwise, <c>false</c>.</returns>
    private async Task<bool> HasAccess<T>(T obj, RepositoryOperationEnum operation, CancellationToken cancellationToken) where T : class
    {
        if (_info.IsAdmin())
        {
            return true;
        }

        if (_protection.FirstOrDefault(x => x.IsMatch(typeof(T))) is IProtected<T> entityLock)
        {
            return await entityLock.HasAccess(obj, _info.GetInternalUserId(), operation, cancellationToken);
        }

        return true;
    }
}

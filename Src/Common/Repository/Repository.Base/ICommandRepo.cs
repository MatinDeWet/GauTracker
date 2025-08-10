namespace Repository.Base;

/// <summary>
/// Defines command operations for data modifications.
/// </summary>
public interface ICommandRepo
{
    /// <summary>
    /// Stages entity for insertion.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="obj">The entity to insert.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task InsertAsync<T>(T obj, CancellationToken cancellationToken) where T : class;

    /// <summary>
    /// Stages entity for insertion, persisting immediately.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="obj">The entity to insert.</param>
    /// <param name="persistImmediately">Whether to save changes immediately.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task InsertAsync<T>(T obj, bool persistImmediately, CancellationToken cancellationToken) where T : class;

    /// <summary>
    /// Stages entity for update.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="obj">The entity to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UpdateAsync<T>(T obj, CancellationToken cancellationToken) where T : class;

    /// <summary>
    /// Stages entity for update, persisting immediately.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="obj">The entity to update.</param>
    /// <param name="persistImmediately">Whether to save changes immediately.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UpdateAsync<T>(T obj, bool persistImmediately, CancellationToken cancellationToken) where T : class;

    /// <summary>
    /// Stages entity for deletion.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="obj">The entity to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteAsync<T>(T obj, CancellationToken cancellationToken) where T : class;

    /// <summary>
    /// Stages entity for deletion, persisting immediately.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="obj">The entity to delete.</param>
    /// <param name="persistImmediately">Whether to save changes immediately.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteAsync<T>(T obj, bool persistImmediately, CancellationToken cancellationToken) where T : class;

    /// <summary>
    /// Persists all staged changes to the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SaveAsync(CancellationToken cancellationToken);
}

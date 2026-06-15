namespace Repository.Contracts;

/// <summary>
/// Defines secure command operations with built-in permission validation for data modifications.
/// </summary>
public interface ISecureCommandRepo : ICommandRepo
{
    /// <summary>
    /// Validates permissions and stages the entity for insertion.
    /// This is the secure version of <see cref="ICommandRepo.InsertAsync{T}(T, CancellationToken)"/> with built-in permission validation.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="obj">The entity to insert.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user does not have permission to insert the entity.</exception>
    new Task InsertAsync<T>(T obj, CancellationToken cancellationToken) where T : class;

    /// <summary>
    /// Validates permissions and stages the entity for insertion, optionally persisting immediately.
    /// This is the secure version of <see cref="ICommandRepo.InsertAsync{T}(T, bool, CancellationToken)"/> with built-in permission validation.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="obj">The entity to insert.</param>
    /// <param name="persistImmediately">Whether to save changes immediately.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user does not have permission to insert the entity.</exception>
    new Task InsertAsync<T>(T obj, bool persistImmediately, CancellationToken cancellationToken) where T : class;

    /// <summary>
    /// Validates permissions and stages the entity for update.
    /// This is the secure version of <see cref="ICommandRepo.UpdateAsync{T}(T, CancellationToken)"/> with built-in permission validation.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="obj">The entity to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user does not have permission to update the entity.</exception>
    new Task UpdateAsync<T>(T obj, CancellationToken cancellationToken) where T : class;

    /// <summary>
    /// Validates permissions and stages the entity for update, optionally persisting immediately.
    /// This is the secure version of <see cref="ICommandRepo.UpdateAsync{T}(T, bool, CancellationToken)"/> with built-in permission validation.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="obj">The entity to update.</param>
    /// <param name="persistImmediately">Whether to save changes immediately.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user does not have permission to update the entity.</exception>
    new Task UpdateAsync<T>(T obj, bool persistImmediately, CancellationToken cancellationToken) where T : class;

    /// <summary>
    /// Validates permissions and stages the entity for deletion.
    /// This is the secure version of <see cref="ICommandRepo.DeleteAsync{T}(T, CancellationToken)"/> with built-in permission validation.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="obj">The entity to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user does not have permission to delete the entity.</exception>
    new Task DeleteAsync<T>(T obj, CancellationToken cancellationToken) where T : class;

    /// <summary>
    /// Validates permissions and stages the entity for deletion, optionally persisting immediately.
    /// This is the secure version of <see cref="ICommandRepo.DeleteAsync{T}(T, bool, CancellationToken)"/> with built-in permission validation.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="obj">The entity to delete.</param>
    /// <param name="persistImmediately">Whether to save changes immediately.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user does not have permission to delete the entity.</exception>
    new Task DeleteAsync<T>(T obj, bool persistImmediately, CancellationToken cancellationToken) where T : class;
}

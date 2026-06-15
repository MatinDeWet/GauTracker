namespace Repository.Contracts;

/// <summary>
/// Defines secure query operations with automatic security filtering based on user permissions.
/// </summary>
public interface ISecureQueryRepo : IQueryRepo
{
    /// <summary>
    /// Returns a queryable collection with security filters applied based on the current user's permissions.
    /// This is the secure version of <see cref="IQueryRepo.GetQueryable{T}"/> with automatic security filtering.
    /// </summary>
    /// <typeparam name="T">The entity type to query.</typeparam>
    /// <returns>A filtered <see cref="IQueryable{T}"/> or unfiltered if no protection is configured.</returns>
    new IQueryable<T> GetQueryable<T>() where T : class;
}

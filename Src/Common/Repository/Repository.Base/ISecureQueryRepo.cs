namespace Repository.Base;
/// <summary>
/// Defines secure query operations with automatic security filtering based on user permissions.
/// </summary>
public interface ISecureQueryRepo : IQueryRepo
{
    /// <summary>
    /// Returns a queryable collection with security filters applied based on the current user's permissions.
    /// </summary>
    /// <typeparam name="T">The entity type to query.</typeparam>
    /// <returns>A filtered <see cref="IQueryable{T}"/> or unfiltered if no protection is configured.</returns>
    IQueryable<T> Secure<T>() where T : class;
}

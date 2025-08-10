using Gautrain.Integration.Api.Models;

namespace Gautrain.Integration.Api;

public interface IGautrainApi
{
    /// <summary>
    /// Gets a list of all Gautrain stations
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of stations</returns>
    Task<List<StationResponse>> GetStationsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets route details between two points
    /// </summary>
    /// <param name="request">Route request parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Route response with details</returns>
    Task<RouteResponse> GetRouteAsync(RouteRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Authenticates with the Gautrain API using username and password
    /// </summary>
    /// <param name="loginRequest">Login credentials</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if authentication successful</returns>
    Task<bool> LoginAsync(LoginRequest loginRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets transit card details for the authenticated user
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Transit card information</returns>
    Task<TransitCardResponse> GetTransitCardsAsync(CancellationToken cancellationToken = default);
}
